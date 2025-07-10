using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;
using TravelFinalProject.ViewModels;

namespace TravelFinalProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DestinationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DestinationController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        //public async Task<IActionResult> Index(string langCode = "en")
        //{
        //    List<GetDestinationVM> destinationVMs = await _context.Destinations.Include(m => m.DestinationTranslations.Where(t => t.LangCode == langCode)).Include(d => d.Category).Select(d => new GetDestinationVM
        //    {
        //        Name = d.DestinationTranslations.FirstOrDefault().Name,
        //        Id = d.Id,
        //        Description = d.DestinationTranslations.FirstOrDefault().Description,
        //        Address = d.DestinationTranslations.FirstOrDefault().Address,
        //        City = d.DestinationTranslations.FirstOrDefault().City,
        //        Country = d.DestinationTranslations.FirstOrDefault().Country,
        //        CategoryName = d.Category.DestinationCategoryTranslations.FirstOrDefault().Name,
        //        Price = d.Price,
        //        MainImage = d.DestinationImages.FirstOrDefault(di => di.IsPrimary == true).Image,

        //    }).ToListAsync();
        //    return View(destinationVMs);
        //}

        public async Task<IActionResult> Index(string langCode = "en", int page = 1)
        {
            int pageSize = 6;

            var query = _context.Destinations
                .Include(m => m.DestinationTranslations.Where(t => t.LangCode == langCode))
                .Include(d => d.Category).ThenInclude(c => c.DestinationCategoryTranslations)
                .Include(d => d.DestinationImages)
                .AsQueryable();

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            if (totalPages == 0) totalPages = 1;

            if (page < 1 || page > totalPages)
                return BadRequest("Səhv sorğu: səhifə nömrəsi düzgün deyil.");

            var destinationVMs = await query
                .OrderBy(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new GetDestinationVM
                {
                    Name = d.DestinationTranslations.FirstOrDefault().Name,
                    Id = d.Id,
                    Description = d.DestinationTranslations.FirstOrDefault().Description,
                    Address = d.DestinationTranslations.FirstOrDefault().Address,
                    City = d.DestinationTranslations.FirstOrDefault().City,
                    Country = d.DestinationTranslations.FirstOrDefault().Country,
                    CategoryName = d.Category.DestinationCategoryTranslations.FirstOrDefault().Name,
                    Price = d.Price,
                    MainImage = d.DestinationImages.FirstOrDefault(di => di.IsPrimary == true).Image,
                })
                .ToListAsync();

            var paginatedVM = new PaginatedVM<GetDestinationVM>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = destinationVMs,
                LangCode = langCode
            };

            return View(paginatedVM);
        }


        public async Task<IActionResult> Create()
        {

            CreateDestinationVM destinationVM = new CreateDestinationVM
            {
                DestinationCategories = await _context.DestinationCategoryTranslations.ToListAsync()
            };
            return View(destinationVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDestinationVM destinationVM, string langCode = "en")
        {
            destinationVM.DestinationCategories = await _context.DestinationCategoryTranslations
        .Where(t => t.LangCode == langCode)
        .ToListAsync();
            destinationVM.Categories = await _context.DestinationCategories.Include(m => m.DestinationCategoryTranslations.Where(t => t.LangCode == langCode)).ToListAsync();
            if (!ModelState.IsValid) return View(destinationVM);


            if (!destinationVM.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateDestinationVM.MainPhoto), "Photo type is wrong.");
                return View(destinationVM);
            }
            if (!destinationVM.MainPhoto.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateDestinationVM.MainPhoto), "File size cannot exceed 2 MB.");
                return View(destinationVM);
            }
            bool exists = await _context.DestinationTranslations
    .AnyAsync(t => t.Name.ToLower() == destinationVM.Name.ToLower() && t.LangCode == destinationVM.LangCode);


            if (exists)
            {
                ModelState.AddModelError(nameof(CreateDestinationVM.Name), "Destination with this name already exists.");
                return View(destinationVM);
            }

            DestinationImage main = new DestinationImage
            {
                Image = await destinationVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                IsPrimary = true,
                CreatedAt = DateTime.Now
            };
            Destination destination = new Destination
            {
                Price = destinationVM.Price,
                IsFeatured = destinationVM.IsFeatured,
                CategoryId = destinationVM.CategoryId,
                DestinationImages = new List<DestinationImage> { main }

            };
            destination.DestinationImages.Add(main);
            await _context.Destinations.AddAsync(destination);
            await _context.SaveChangesAsync();
            DestinationTranslation translation = new DestinationTranslation
            {
                LangCode = destinationVM.LangCode,
                Name = destinationVM.Name,
                Description = destinationVM.Description,
                Country = destinationVM.Country,
                City = destinationVM.City,
                Address = destinationVM.Address,
                DestinationId = destination.Id
            };
            if (destinationVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;
                foreach (var photo in destinationVM.AdditionalPhotos)
                {
                    if (!photo.ValidateType("image/"))
                    {
                        text += $"<p>{photo.FileName} named image type is incorrect</p>";
                        continue;
                    }
                    if (!photo.ValidateSize(FileSize.MB, 2))
                    {
                        text += $"<p>{photo.FileName} named image  is oversized</p>";
                        continue;
                    }
                    destination.DestinationImages.Add(new DestinationImage
                    {
                        Image = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                        IsPrimary = null,
                        CreatedAt = DateTime.Now

                    });
                }
                TempData["FileWarning"] = text;
            }
            await _context.DestinationTranslations.AddAsync(translation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Update(int? id)
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Destination? destination = await _context.Destinations
                   .Include(m => m.Category)
                   .Include(m => m.DestinationTranslations)
                   .Include(d => d.DestinationImages)
                   .FirstOrDefaultAsync(d => d.Id == id);
            if (destination == null) return NotFound();
            var translation = destination.DestinationTranslations
        .FirstOrDefault(t => t.LangCode == langCode);
            UpdateDestinationVM destinationVM = new UpdateDestinationVM
            {

                Name = destination.DestinationTranslations.FirstOrDefault().Name,
                Description = destination.DestinationTranslations.FirstOrDefault().Description,
                Country = destination.DestinationTranslations.FirstOrDefault().Country,
                CategoryId = destination.CategoryId,
                Address = destination.DestinationTranslations.FirstOrDefault().Address,
                City = destination.DestinationTranslations.FirstOrDefault().City,
                LangCode = destination.DestinationTranslations.FirstOrDefault().LangCode,
                Price = destination.Price,
                IsFeatured = destination.IsFeatured,
                PrimaryImage = destination.DestinationImages.FirstOrDefault(pi => pi.IsPrimary == true).Image,
                Categories = await _context.DestinationCategories.ToListAsync(),
                DestinationCategories = await _context.DestinationCategoryTranslations.Where(t => t.LangCode == langCode).ToListAsync()

            };
            return View(destinationVM);


        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateDestinationVM destinationVM, int id)
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            destinationVM.Categories = await _context.DestinationCategories.Include(d => d.DestinationCategoryTranslations).ToListAsync();
            destinationVM.DestinationCategories = await _context.DestinationCategoryTranslations.ToListAsync();
            if (!ModelState.IsValid) return View(destinationVM);

            Destination? existed = await _context.Destinations.Include(d => d.DestinationImages).Include(m => m.DestinationTranslations).FirstOrDefaultAsync(d => d.Id == id);
            if (existed is null) return NotFound();
            if (destinationVM.MainPhoto is not null)
            {
                if (!destinationVM.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(CreateDestinationVM.MainPhoto), "Photo type is wrong.");
                    return View(destinationVM);
                }
                if (!destinationVM.MainPhoto.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(CreateDestinationVM.MainPhoto), "File size cannot exceed 2 MB.");
                    return View(destinationVM);
                }

            }
            bool result = _context.DestinationCategories.Any(c => c.Id == destinationVM.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(UpdateDestinationVM.CategoryId), "Category doesn't exist");
                return View(destinationVM);
            }

            bool nameExists = await _context.DestinationTranslations
    .AnyAsync(t => t.Name.ToLower() == destinationVM.Name.ToLower() && t.LangCode == destinationVM.LangCode);
            if (nameExists)
            {
                ModelState.AddModelError(nameof(UpdateDestinationVM.Name), "Bu adla başqa destination artıq mövcuddur.");
                return View(destinationVM);
            }

            if (destinationVM.MainPhoto != null)
            {
                DestinationImage main = new DestinationImage
                {
                    IsPrimary = true,
                    Image = await destinationVM.MainPhoto.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                    CreatedAt = DateTime.Now
                };
                DestinationImage? exitedMain = existed.DestinationImages.FirstOrDefault(p => p.IsPrimary == true);
                exitedMain.Image.DeleteFile(_env.WebRootPath, "assets", "images", "trending");
                existed.DestinationImages.Remove(exitedMain);
                existed.DestinationImages.Add(main);
            }
            existed.DestinationTranslations.FirstOrDefault().Name = destinationVM.Name;
            existed.DestinationTranslations.FirstOrDefault().Description = destinationVM.Description;
            existed.DestinationTranslations.FirstOrDefault().Country = destinationVM.Country;
            existed.DestinationTranslations.FirstOrDefault().LangCode = destinationVM.LangCode;
            existed.Price = destinationVM.Price;
            existed.DestinationTranslations.FirstOrDefault().Address = destinationVM.Address;
            existed.DestinationTranslations.FirstOrDefault().City = destinationVM.City;
            existed.CategoryId = destinationVM.CategoryId;
            existed.IsFeatured = destinationVM.IsFeatured;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1)
            {
                return BadRequest();
            }
            Destination? destination = await _context.Destinations.Include(d => d.DestinationTranslations).Include(d => d.DestinationImages).FirstOrDefaultAsync(d => d.Id == id);
            if (destination is null) return NotFound();
            foreach (DestinationImage desImage in destination.DestinationImages)
            {
                desImage.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            }
            _context.Destinations.Remove(destination);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
