using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.ViewModels;


namespace TravelFinalProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ReviewController : Controller
    {
        private readonly AppDbContext _context;

        public ReviewController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            int pageSize = 3;

            var query = _context.Reviews
         .Include(r => r.ReviewTranslations)
         .Include(r => r.User)
         .Select(r => new ReviewAdminVM
         {
             Id = r.Id,
             TourId = r.TourId,
             UserName = r.User.UserName,
             UserImage = r.User.Image ?? "ImagePP.webp",
             Rating = r.Rating,
             LangCode = r.LangCode,
             Comment = r.ReviewTranslations.FirstOrDefault().Comment,
             IsApproved = r.IsApproved,
             CreatedAt = r.CreatedAt,
         });


            int count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1 || page > totalPages) return BadRequest();
            var reviews = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var paginatedVM = new PaginatedVM<ReviewAdminVM>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = reviews
            };

            return View(paginatedVM);
        }






        [HttpPost]
        public async Task<IActionResult> UpdateReview(int id, bool isApproved, string langCode)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null) return NotFound();

            review.IsApproved = isApproved;
            review.LangCode = langCode;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



    }


}


