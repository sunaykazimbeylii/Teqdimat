using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.ViewModels;

namespace TravelFinalProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DestinationCategoryController : Controller
    {
        private readonly AppDbContext _context;

        public DestinationCategoryController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string langCode = "en")
        {
            List<GetDestinationCategoryVM> categoryVM = await _context.DestinationCategories.Include(m => m.DestinationCategoryTranslations.Where(t => t.LangCode == langCode)).Select(d => new GetDestinationCategoryVM
            {
                Id = d.Id,
                Name = d.DestinationCategoryTranslations.FirstOrDefault().Name,
            }).ToListAsync();
            return View(categoryVM);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDestinationCategoryVM categoryVM)
        {
            if (!ModelState.IsValid) return View(categoryVM);


            bool result = await _context.DestinationCategories.AnyAsync(c => c.DestinationCategoryTranslations.FirstOrDefault().Name == categoryVM.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(CreateDestinationCategoryVM.Name), $"{categoryVM.Name} Bu adda category var");
                return View();

            }
            DestinationCategory destinationCategory = new DestinationCategory
            {
                CreatedAt = DateTime.Now,

            };

            await _context.DestinationCategories.AddAsync(destinationCategory);
            await _context.SaveChangesAsync();
            DestinationCategoryTranslation translation = new DestinationCategoryTranslation
            {
                LangCode = categoryVM.LangCode,
                Name = categoryVM.Name,
                DestinationCategoryId = destinationCategory.Id,
            };
            await _context.DestinationCategoryTranslations.AddAsync(translation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Update(int? id, string langCode = "en")
        {
            DestinationCategory category = await _context.DestinationCategories.Include(dc => dc.DestinationCategoryTranslations.Where(d => d.LangCode == langCode)).FirstOrDefaultAsync(c => c.Id == id);
            if (category is null) return NotFound();

            UpdateDestinationCategoryVM categoryVM = new UpdateDestinationCategoryVM
            {

                Name = category.DestinationCategoryTranslations.FirstOrDefault().Name,


            };

            return View(categoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateDestinationCategoryVM categoryVM, int? id)
        {
            if (!ModelState.IsValid) return View();
            bool result = await _context.DestinationCategories.AnyAsync(d => d.DestinationCategoryTranslations.FirstOrDefault().Name == categoryVM.Name && d.Id != id);

            if (result)
            {
                ModelState.AddModelError(nameof(UpdateDestinationCategoryVM.Name), $"{categoryVM.Name} adli category var");
                return View();
            }
            DestinationCategory existed = await _context.DestinationCategories.Include(d => d.DestinationCategoryTranslations).FirstOrDefaultAsync(d => d.Id == id);
            if (existed == null) return NotFound();
            if (existed.DestinationCategoryTranslations.FirstOrDefault().Name == categoryVM.Name) return RedirectToAction(nameof(Index));
            existed.DestinationCategoryTranslations.FirstOrDefault().Name = categoryVM.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            DestinationCategory category = await _context.DestinationCategories.Include(d => d.DestinationCategoryTranslations).FirstOrDefaultAsync(d => d.Id == id);
            if (category is null) return NotFound();
            _context.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

    }
}

