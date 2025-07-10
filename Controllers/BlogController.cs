using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Claims;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities.Enums;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels;
using TravelFinalProject.ViewModels.BlogVM;


namespace TravelFinalProject.Controllers
{

    public class BlogController : Controller
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Blog(int page = 1, int key = 1)
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            int pageSize = 3;
            var query = _context.Blogs
                .Include(b => b.BlogTranslations)
                .AsQueryable();

            query = query.Where(b => b.BlogTranslations.Any(t => t.LangCode == langCode));

            // SortType əsasında sıralama
            switch (key)
            {
                case (int)SortType.Date:
                    query = query.OrderByDescending(b => b.PublishedDate);
                    break;
                case (int)SortType.Rating:
                    query = query.OrderByDescending(b => b.IsPopular);
                    break;
                default:
                    query = query.OrderBy(b => b.Id);
                    break;
            }

            int count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1 || page > totalPages)
                throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");

            var pagedBlogs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BlogVM
                {
                    Id = b.Id,
                    ImageUrl = b.ImageUrl,
                    PublishedDate = b.PublishedDate,
                    Title = b.BlogTranslations.FirstOrDefault().Title,
                    Content = b.BlogTranslations.FirstOrDefault().Content
                })
                .ToListAsync();

            var paginatedVM = new PaginatedVM<BlogVM>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = pagedBlogs
            };

            return View(paginatedVM);
        }

        public async Task<IActionResult> BlogDetail(int id)
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            var blog = await _context.Blogs
                .Include(b => b.BlogTranslations)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null)
                throw new NotFoundException("tapılmadı");

            var translation = blog.BlogTranslations.FirstOrDefault(t => t.LangCode == langCode)
                              ?? blog.BlogTranslations.FirstOrDefault();

            var blogDetailVM = new BlogDetailVM
            {
                Id = blog.Id,
                ImageUrl = blog.ImageUrl,
                PublishedDate = blog.PublishedDate,
                Title = translation?.Title ?? "No Title",
                Comment = translation?.Content ?? "No Content",
                Reviews = blog.Reviews.Select(r => new BlogReviewVM
                {
                    Id = r.Id,
                    UserName = r.User?.UserName ?? "Naməlum",
                    UserImage = r.User?.Image ?? "default.png",
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                }).ToList()
            };

            return View(blogDetailVM);
        }




        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(BlogReviewCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("BlogDetail", new { id = model.BlogId });
            }

            var blog = await _context.Blogs.FindAsync(model.BlogId);
            if (blog == null)
            {
                throw new NotFoundException("tapilmadi");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var review = new BlogReview
            {
                BlogId = model.BlogId,
                UserId = userId,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.BlogReviews.Add(review);
            await _context.SaveChangesAsync();

            return RedirectToAction("BlogDetail", new { id = model.BlogId });
        }



    }
}
