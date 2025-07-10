using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels.ReviewVM;

namespace TravelFinalProject.Controllers
{
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ReviewAdd(int tourId, string langCode = "en")
        {
            var reviews = await _context.Reviews
    .Include(r => r.User)
    .ToListAsync();
            var tour = await _context.Tours.Include(t => t.TourTranslations.Where(m => m.LangCode == langCode)).FirstOrDefaultAsync(t => t.Id == tourId);
            if (tour == null) throw new NotFoundException("tapilmadi");

            var vm = new ReviewVM
            {
                TourId = tour.Id,
                TourTitle = tour.TourTranslations.FirstOrDefault().Title,

            };

            return View(vm);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ReviewAdd(ReviewVM vm)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            bool alreadyReviewed = await _context.Reviews.Include(r => r.ReviewTranslations)
        .AnyAsync(r => r.UserId == userId && r.TourId == vm.TourId);

            if (alreadyReviewed)
            {
                ModelState.AddModelError("", "Siz bu tur üçün artıq rəy yazmısınız.");
                var tour = await _context.Tours.Include(t => t.TourTranslations).FirstOrDefaultAsync(tt => tt.Id == vm.TourId);
                vm.TourTitle = tour?.TourTranslations.FirstOrDefault().Title ?? "";
                return View(vm);
            }

            if (!ModelState.IsValid)
            {
                var tour = await _context.Tours.Include(t => t.TourTranslations).FirstOrDefaultAsync(tt => tt.Id == vm.TourId);
                vm.TourTitle = tour?.TourTranslations.FirstOrDefault().Title ?? "";
                return View(vm);
            }
            var review = new Review
            {
                UserId = userId,
                LangCode = vm.LengCode,
                Rating = vm.Rating,
                TourId = vm.TourId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            ReviewTranslation translation = new ReviewTranslation
            {
                Comment = vm.Comment,

                ReviewId = review.Id

            };
            _context.ReviewTranslations.Add(translation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ThankYou));
        }

        public async Task<IActionResult> ThankYou()
        {
            return View();
        }
    }
}



