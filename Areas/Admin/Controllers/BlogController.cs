using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;
using TravelFinalProject.ViewModels;
using TravelFinalProject.ViewModels.BlogVM;

namespace TravelFinalProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(string langCode = "en", int page = 1)
        {
            int pageSize = 5;

            var query = _context.Blogs
                .Include(b => b.BlogTranslations.Where(bt => bt.LangCode == langCode))
                .AsQueryable();

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1 || page > totalPages)
            {
                return BadRequest("Səhv sorğu: səhifə nömrəsi düzgün deyil.");
            }

            var blogVMs = await query
                .OrderByDescending(b => b.PublishedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BlogVM
                {
                    Id = b.Id,
                    ImageUrl = b.ImageUrl,
                    PublishedDate = b.PublishedDate,
                    Content = b.BlogTranslations.FirstOrDefault().Content,
                    IsPopular = b.IsPopular,
                    Title = b.BlogTranslations.FirstOrDefault().Title
                })
                .ToListAsync();

            var paginatedVM = new PaginatedVM<BlogVM>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = blogVMs,
                LangCode = langCode
            };

            return View(paginatedVM);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBlogVM model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            if (!model.Photo.ValidateType("image"))
            {
                ModelState.AddModelError(nameof(CreateBlogVM.Photo), "Yalnız şəkil faylları yükləyə bilərsiniz.");
                return View(model);
            }

            if (!model.Photo.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateBlogVM.Photo), "Faylın ölçüsü 2MB-dan çox ola bilməz.");
                return View(model);
            }

            string fileName = await model.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "blog");

            var blog = new Blog
            {
                ImageUrl = fileName,
                PublishedDate = model.PublishedDate,
                IsPopular = model.IsPopular,
                CreatedAt = DateTime.Now
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            var translation = new BlogTranslation
            {
                LangCode = model.LangCode,
                Title = model.Title,
                Content = model.Content,
                BlogId = blog.Id

            };

            _context.BlogTranslations.Add(translation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Update(int? id, string lengCode = "en")
        {
            if (id is null || id < 1) return BadRequest();

            var blog = await _context.Blogs
                .Include(b => b.BlogTranslations)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            var translation = blog.BlogTranslations.FirstOrDefault();

            var vm = new UpdateBlogVM
            {

                Title = translation?.Title,
                Content = translation?.Content,
                IsPopular = blog.IsPopular,
                PublishedDate = blog.PublishedDate,
                ImageUrl = blog.ImageUrl,


            };

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, UpdateBlogVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var blog = await _context.Blogs
                .Include(b => b.BlogTranslations)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            if (model.Photo != null)
            {
                if (!model.Photo.ValidateType("image"))
                {
                    ModelState.AddModelError(nameof(CreateBlogVM.Photo), "Yalnız şəkil faylları yükləyə bilərsiniz.");
                    return View(model);
                }

                if (!model.Photo.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(CreateBlogVM.Photo), "Faylın ölçüsü 2MB-dan çox ola bilməz.");
                    return View(model);
                }

                blog.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "blog");
                blog.ImageUrl = await model.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "blog");
            }

            blog.PublishedDate = model.PublishedDate;
            blog.IsPopular = model.IsPopular;

            var translation = blog.BlogTranslations.FirstOrDefault();

            if (translation != null)
            {
                translation.Title = model.Title;
                translation.Content = model.Content;
            }
            else
            {
                blog.BlogTranslations.Add(new BlogTranslation
                {

                    Title = model.Title,
                    Content = model.Content
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }




        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            var blog = await _context.Blogs
                .Include(b => b.BlogTranslations)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (blog == null) return NotFound();

            blog.ImageUrl.DeleteFile(_env.WebRootPath, "wwwroot", "images", "blog");
            _context.Remove(blog);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }

}
