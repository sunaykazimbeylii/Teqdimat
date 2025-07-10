using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels.ProfileVM;
using TravelFinalProject.ViewModels.Users;

namespace TravelFinalProject.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }
        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) throw new NotFoundException("tapilmadi");
            var model = new ProfileVM
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Image = user.Image,
                Bio = user.Bio,
                City = user.City,
                Country = user.Country,
                ChangePassword = new ChangePasswordVM()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(ProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                throw new NotFoundException("tapilmadi");

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.PhoneNumber = model.PhoneNumber;
            user.Bio = model.Bio;
            user.City = model.City;
            user.Country = model.Country;

            if (model.Photo != null)
            {
                if (!model.Photo.ValidateType("image"))
                {
                    ModelState.AddModelError("Photo", "Yalnız şəkil faylı yükləyə bilərsiniz.");
                    return View(model);
                }

                if (!model.Photo.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError("Photo", "Şəklin ölçüsü maksimum 2MB ola bilər.");
                    return View(model);
                }

                string fileName = await model.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending");
                if (!string.IsNullOrEmpty(user.Image) && user.Image != "ImagePP.webp")
                {
                    user.Image.DeleteFile(_env.WebRootPath, "assets", "images", "trending");
                }
                user.Image = fileName;
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                return View(model);
            }

            if (!string.IsNullOrEmpty(model.ChangePassword.CurrentPassword) &&
                !string.IsNullOrEmpty(model.ChangePassword.NewPassword) &&
                !string.IsNullOrEmpty(model.ChangePassword.ConfirmPassword))
            {
                if (model.ChangePassword.NewPassword != model.ChangePassword.ConfirmPassword)
                {
                    ModelState.AddModelError("ChangePassword.ConfirmPassword", "Passwords do not match.");
                    return View(model);
                }

                var passwordResult = await _userManager.ChangePasswordAsync(user, model.ChangePassword.CurrentPassword, model.ChangePassword.NewPassword);

                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    return View(model);
                }
            }

            TempData["Status"] = "Profil və şifrə uğurla yeniləndi.";
            model.Image = user.Image;

            return View(model);
        }





    }
}


