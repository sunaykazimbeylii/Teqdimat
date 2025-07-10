using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities.Enums;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels;
using TravelFinalProject.ViewModels.Users;
using ResetPasswordVM = TravelFinalProject.ViewModels.Users.ResetPasswordVM;

namespace TravelFinalProject.Controllers
{

    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }


        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser user = new AppUser()
            {
                Name = registerVM.Name,
                Surname = registerVM.Surname,
                UserName = registerVM.UserName,
                Email = registerVM.Email,
                City = registerVM.City,
                Country = registerVM.Country,
                DateOfBirth = registerVM.BirthDate,
                PhoneNumber = registerVM.PhoneNumber

            };
            IdentityResult result = await _userManager.CreateAsync(user, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                };
                return View();
            }
            await _userManager.AddToRoleAsync(user, UserRole.Member.ToString());
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, Email = user.Email }, Request.Scheme);
            string emailBody = $"<p>Please confirm your account by clicking the link below:</p><p><a href='{confirmationLink}'>Confirm Email</a></p>";

            await _emailService.SendMailAsync(user.Email, "Email Confirmation", emailBody, isHtml: true);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new NotFoundException("Tələb olunan məlumat tapılmadı.");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");
            await _signInManager.SignInAsync(user, isPersistent: false);
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginVM.UserNameOrEmail || u.Email == loginVM.UserNameOrEmail);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Username,Email or Password is incorrect");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(user, loginVM.Password, loginVM.IsPersistent, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "You are blocked");
                return View();
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm your email");
                return View();
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Username,Email or Password is incorrect");
                return View();

            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }





        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");


        }
        public async Task<IActionResult> CreateRole()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                if (!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString()

                    });

                }
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");

        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Account", new { token = Uri.EscapeDataString(token), email = model.Email }, Request.Scheme);

                string emailBody = $"Please reset your password by clicking here: <a href='{resetLink}'>link</a>";
                await _emailService.SendMailAsync(model.Email, "Reset your password", emailBody, true);
            }
            ViewBag.Message = "If your email is registered, you will receive a password reset link.";

            return View();
        }

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MailMessage();
            message.To.Add(to);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            message.From = new MailAddress("kazimbeyliii@gmail.com", "TravelApp");

            using var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("kazimbeyliii@gmail.com", "xuzskszbukamltfn"),
                EnableSsl = true
            };


            await smtp.SendMailAsync(message);
        }
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token)) throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new NotFoundException("tapilmadi");

            var model = new ResetPasswordVM
            {
                Email = email,
                Token = token
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {

            if (string.IsNullOrWhiteSpace(model.Token)) throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) throw new NotFoundException("tapilmadi");


            var decodedToken = Uri.UnescapeDataString(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, Request.Scheme);

            string emailBody = $"Please confirm your email by clicking this link: <a href='{confirmationLink}'>Confirm Email</a>";

            await _emailService.SendMailAsync(user.Email, "Email Confirmation", emailBody, true);

            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }



    }
}
