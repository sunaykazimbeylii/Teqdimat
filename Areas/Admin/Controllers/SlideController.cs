using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;
using TravelFinalProject.ViewModels;

namespace TravelFinalProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class SlideController : Controller

    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index(string langCode = "en")
        {
            List<GetSlideVM> slideVMs = await _context.Slides.Include(s => s.SlideTranslations.Where(t => t.LangCode == langCode)).Where(s => s.IsActive == true).Select(s =>

                new GetSlideVM
                {
                    Id = s.Id,
                    Title = s.SlideTranslations.FirstOrDefault().Title,
                    Subtitle = s.SlideTranslations.FirstOrDefault().Subtitle,
                    ButtonText = s.SlideTranslations.FirstOrDefault().ButtonText,
                    ButtonUrl = s.ButtonUrl,
                    IsActive = s.IsActive,
                    ImageUrl = s.ImageUrl,
                    CreatedAt = s.CreatedAt,
                    Order = s.Order,

                }
            ).ToListAsync();

            return View(slideVMs);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSlideVM slideVM)
        {

            var slides = await _context.Slides.Include(s => s.SlideTranslations).ToListAsync();
            if (!ModelState.IsValid)
            {
                return View(slides);
            }


            if (!slideVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File type is incorrect");
                return View();
            }
            if (!slideVM.Photo.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Photo), "File size sould be less than 2MB");
                return View();
            }
            bool result = await _context.Slides.Include(st => st.SlideTranslations)
     .AnyAsync(s => s.Order == slideVM.Order && s.SlideTranslations.FirstOrDefault().LangCode == slideVM.LangCode);

            if (result)
            {
                ModelState.AddModelError(nameof(CreateSlideVM.Order), $"{slideVM.Order} This order value already exists for selected language");
                return View();
            }
            string fileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
            Slide slide = new Slide
            {

                ButtonUrl = slideVM.ButtonUrl,
                IsActive = slideVM.IsActive,
                Order = slideVM.Order,
                ImageUrl = fileName,
                CreatedAt = DateTime.Now
            };

            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            SlideTranslation translation = new SlideTranslation
            {
                LangCode = slideVM.LangCode,
                Title = slideVM.Title,
                Subtitle = slideVM.Subtitle,
                ButtonText = slideVM.ButtonText,
                SlideId = slide.Id
            };
            await _context.SlideTranslations.AddAsync(translation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Slide? slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide is null) return NotFound();
            slide.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
            _context.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id, string langCode = "en")
        {
            Slide? slide = await _context.Slides
    .Include(s => s.SlideTranslations)
    .FirstOrDefaultAsync(s => s.Id == id);

            if (slide is null) return NotFound();

            var translation = slide.SlideTranslations.FirstOrDefault(t => t.LangCode == langCode);
            if (translation == null)
            {

                translation = slide.SlideTranslations.FirstOrDefault(t => t.LangCode == "en")
                              ?? slide.SlideTranslations.FirstOrDefault();
            }

            UpdateSlideVM slideVM = new UpdateSlideVM
            {
                Title = translation?.Title ?? "",
                Order = slide.Order,
                Subtitle = translation?.Subtitle ?? "",
                Image = slide.ImageUrl,
                ButtonText = translation?.ButtonText ?? "",
                ButtonUrl = slide.ButtonUrl,
                IsActive = slide.IsActive,
            };

            return View(slideVM);

            //    if (id is null || id < 1) return BadRequest();
            //    Slide? slide = await _context.Slides.Include(s => s.SlideTranslations.Where(t => t.LangCode == langCode)).FirstOrDefaultAsync(s => s.Id == id);
            //    if (slide is null) return NotFound();
            //    UpdateSlideVM slideVM = new UpdateSlideVM
            //    {

            //        Title = slide.SlideTranslations.FirstOrDefault().Title,
            //        Order = slide.Order,
            //        Subtitle = slide.SlideTranslations.FirstOrDefault().Subtitle,
            //        Image = slide.ImageUrl,
            //        ButtonText = slide.SlideTranslations.FirstOrDefault().ButtonText,
            //        ButtonUrl = slide.ButtonUrl,
            //        IsActive = slide.IsActive,

            //    };
            //    return View(slideVM);
            //}
        }
        [HttpPost]

        public async Task<IActionResult> Update(int? id, UpdateSlideVM slideVM)
        {
            if (!ModelState.IsValid) return View(slideVM);
            Slide? existed = await _context.Slides.Include(s => s.SlideTranslations).FirstOrDefaultAsync(s => s.Id == id);
            if (existed is null) return NotFound();
            if (slideVM.Photo is not null)
            {
                if (!slideVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File type is incorrect");
                    return View(slideVM);
                }
                if (!slideVM.Photo.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateSlideVM.Photo), "File size must be less than 2MB");
                    return View(slideVM);

                }
                string fileName = await slideVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "slider");
                existed.ImageUrl.DeleteFile(_env.WebRootPath, "assets", "images", "slider");
                existed.ImageUrl = fileName;
            }
            existed.SlideTranslations.FirstOrDefault().Title = slideVM.Title;
            existed.SlideTranslations.FirstOrDefault().Subtitle = slideVM.Subtitle;
            existed.ButtonUrl = slideVM.ButtonUrl;
            existed.SlideTranslations.FirstOrDefault().ButtonText = slideVM.ButtonText;
            existed.IsActive = slideVM.IsActive;
            existed.Order = slideVM.Order;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
