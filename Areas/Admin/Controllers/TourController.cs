using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels;
using TravelFinalProject.ViewModels.TourVM;

namespace TravelFinalProject.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class TourController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TourController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        //public async Task<IActionResult> Index(string langCode = "en",int page=1)
        //{
        //    int pageSize = 6;

        //    List<GetTourVM> tourVMs = await _context.Tours
        //        .Include(m => m.TourTranslations.Where(t => t.LangCode == langCode)).Include(t => t.TourImages).Include(t => t.Destination)
        //.ThenInclude(d => d.DestinationTranslations.Where(dt => dt.LangCode == langCode)).Select(t => new GetTourVM
        //{
        //    Id = t.Id,
        //    Title = t.TourTranslations.FirstOrDefault().Title,
        //    Description = t.TourTranslations.FirstOrDefault().Description,
        //    Price = t.Price.Value,
        //    Destination = t.Destination,
        //    DestinationId = t.DestinationId,
        //    Duration = t.Duration,
        //    Start_Date = t.Start_Date,
        //    End_Date = t.End_Date,
        //    Available_seats = t.Available_seats,
        //    Location = t.TourTranslations.FirstOrDefault().Location,
        //    Image = t.TourImages.FirstOrDefault(ti => ti.IsPrimary == true).Image,
        //    DestinationName = t.Destination.DestinationTranslations.FirstOrDefault().Name,




        //}).ToListAsync();

        //    return View(tourVMs);
        //}
        public async Task<IActionResult> Index(string langCode = "en", int page = 1)
        {
            int pageSize = 6;

            var query = _context.Tours
                .Include(m => m.TourTranslations.Where(t => t.LangCode == langCode))
                .Include(t => t.TourImages)
                .Include(t => t.Destination)
                    .ThenInclude(d => d.DestinationTranslations.Where(dt => dt.LangCode == langCode));

            int totalCount = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1 || page > totalPages) throw new BadRequestException("Səhv səhifə nömrəsi.");

            List<GetTourVM> tourVMs = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new GetTourVM
                {
                    Id = t.Id,
                    Title = t.TourTranslations.FirstOrDefault().Title,
                    Description = t.TourTranslations.FirstOrDefault().Description,
                    Price = t.Price.Value,
                    Destination = t.Destination,
                    DestinationId = t.DestinationId,
                    Duration = t.Duration,
                    Start_Date = t.Start_Date,
                    End_Date = t.End_Date,
                    Available_seats = t.Available_seats,
                    Location = t.TourTranslations.FirstOrDefault().Location,
                    Image = t.TourImages.FirstOrDefault(ti => ti.IsPrimary == true).Image,
                    DestinationName = t.Destination.DestinationTranslations.FirstOrDefault().Name,
                })
                .ToListAsync();

            var paginatedVM = new PaginatedVM<GetTourVM>
            {
                Items = tourVMs,
                CurrentPage = page,
                TotalPage = totalPages,
                LangCode = langCode
            };

            return View(paginatedVM);
        }

        public async Task<IActionResult> Create()
        {
            CreateTourVM tourVM = new CreateTourVM
            {
                DestinationTranslations = await _context.DestinationTranslations.ToListAsync(),

            };
            return View(tourVM);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateTourVM tourVM)

        {
            tourVM.DestinationTranslations = await _context.DestinationTranslations.ToListAsync();

            tourVM.Destinations = await _context.Destinations.Include(d => d.DestinationTranslations).ToListAsync();

            if (!ModelState.IsValid) return View(tourVM);
            if (!tourVM.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(CreateTourVM.Photo), "Photo type is wrong.");
                return View(tourVM);
            }
            if (!tourVM.Photo.ValidateSize(FileSize.MB, 2))
            {
                ModelState.AddModelError(nameof(CreateTourVM.Photo), "File size cannot exceed 2 MB.");
                return View(tourVM);
            }

            bool Exist = await _context.Tours.AnyAsync(t => t.TourTranslations.FirstOrDefault().Title == tourVM.Title);
            if (Exist)
            {
                ModelState.AddModelError(nameof(CreateTourVM.Title), "This tour title already exists.");
                return View(tourVM);
            }
            if (!await _context.Destinations.AnyAsync(d => d.Id == tourVM.DestinationId))
            {
                ModelState.AddModelError(nameof(CreateTourVM.DestinationId), "Selected destination is invalid.");
                return View(tourVM);
            }
            if (tourVM.End_Date < tourVM.Start_Date)
            {
                ModelState.AddModelError(nameof(CreateTourVM.End_Date), "End date cannot be earlier than the start date.");
                return View(tourVM);
            }

            if (tourVM.Start_Date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                ModelState.AddModelError(nameof(CreateTourVM.Start_Date), "Start date cannot be in the past.");
                return View(tourVM);
            }
            if (tourVM.Start_Date < DateOnly.FromDateTime(DateTime.Now.Date))
            {
                ModelState.AddModelError(nameof(CreateTourVM.Start_Date), "Start date cannot be in the past.");
            }
            bool overlap = await _context.Tours.AnyAsync(t =>
                t.DestinationId == tourVM.DestinationId &&
                t.Start_Date <= tourVM.End_Date &&
                t.End_Date >= tourVM.Start_Date
            );
            bool duplicateTour = await _context.Tours.AnyAsync(t =>
t.DestinationId == tourVM.DestinationId &&
t.Start_Date == tourVM.Start_Date &&
t.End_Date == tourVM.End_Date
);
            if (duplicateTour)
            {
                ModelState.AddModelError("", "Another tour is already planned for this date and location.");
                return View(tourVM);
            }
            if (overlap)
            {
                ModelState.AddModelError("", "Another tour exists to the same destination during the selected period.");
                return View(tourVM);
            }



            TourImage main = new TourImage
            {
                Image = await tourVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                IsPrimary = true,
                CreatedAt = DateTime.Now
            };


            Tour tour = new Tour
            {

                Price = tourVM.Price,
                Duration = tourVM.Duration,
                Start_Date = tourVM.Start_Date,
                End_Date = tourVM.End_Date,
                Available_seats = tourVM.Available_seats.Value,
                TourImages = new List<TourImage> { main },
                DestinationId = tourVM.DestinationId,
                CreatedAt = DateTime.Now
            };

            tour.TourImages.Add(main);
            await _context.Tours.AddAsync(tour);
            await _context.SaveChangesAsync();

            TourTranslation translation = new TourTranslation
            {
                LangCode = tourVM.LangCode,
                TourId = tour.Id,
                Title = tourVM.Title,
                Description = tourVM.Description,
                Location = tourVM.Location
            };
            if (tourVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;
                foreach (var photo in tourVM.AdditionalPhotos)
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
                    tour.TourImages.Add(new TourImage
                    {
                        Image = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                        IsPrimary = null,
                        CreatedAt = DateTime.Now

                    });
                }
                TempData["FileWarning"] = text;
            }
            await _context.TourTranslations.AddAsync(translation);
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
            Tour? tour = await _context.Tours
                .Include(t => t.TourTranslations)
                .Include(t => t.TourImages)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (tour is null) return NotFound();

            var translation = tour.TourTranslations.FirstOrDefault(t => t.LangCode == langCode);


            var mainImage = tour.TourImages.FirstOrDefault(pi => pi.IsPrimary == true)?.Image;

            var tourVM = new UpdateTourVM
            {
                Title = tour.TourTranslations.FirstOrDefault().Title,
                Description = tour.TourTranslations.FirstOrDefault().Description,
                Location = tour.TourTranslations.FirstOrDefault().Location,
                Price = tour.Price,
                Duration = tour.Duration,
                Start_Date = tour.Start_Date,
                End_Date = tour.End_Date,
                Available_seats = tour.Available_seats,
                DestinationId = tour.DestinationId,
                Destinations = await _context.Destinations
                    .Include(d => d.DestinationTranslations)
                    .ToListAsync(),
                Image = mainImage,
                TourImages = tour.TourImages.Where(ti => ti.IsPrimary == null).ToList(),
                DestinationTranslations = await _context.DestinationTranslations.ToListAsync()
            };

            return View(tourVM);
        }


        [HttpPost]
        public async Task<IActionResult> Update(UpdateTourVM tourVM, int id)
        {
            tourVM.Destinations = await _context.Destinations.Include(d => d.DestinationTranslations).ToListAsync();

            if (!ModelState.IsValid) return View(tourVM);

            Tour? existTour = await _context.Tours.Include(t => t.TourTranslations).Include(t => t.TourImages).FirstOrDefaultAsync(t => t.Id == id);
            if (existTour is null) return NotFound();
            bool duplicateTour = await _context.Tours.AnyAsync(t =>
    t.Id != id && // Mövcud tura düzəliş edəndə onu yox say
    t.DestinationId == tourVM.DestinationId &&
    t.Start_Date == tourVM.Start_Date &&
    t.End_Date == tourVM.End_Date
);
            if (duplicateTour)
            {
                ModelState.AddModelError("", "Seçilmiş tarixlərdə bu məntəqəyə aid başqa bir tur artıq mövcuddur.");
                return View(tourVM);
            }
            if (!await _context.Destinations.AnyAsync(d => d.Id == tourVM.DestinationId))
            {
                ModelState.AddModelError(nameof(UpdateTourVM.DestinationId), "Selected destination is invalid.");
                return View(tourVM);
            }

            if (tourVM.End_Date < tourVM.Start_Date)
            {
                ModelState.AddModelError(nameof(UpdateTourVM.End_Date), "End date cannot be earlier than start date.");
                return View(tourVM);
            }

            if (tourVM.Start_Date < DateOnly.FromDateTime(DateTime.UtcNow.Date))
            {
                ModelState.AddModelError(nameof(UpdateTourVM.Start_Date), "Start date cannot be in the past.");
                return View(tourVM);
            }
            if (tourVM.PhotoIds is null)
            {
                tourVM.PhotoIds = new();
            }
            List<TourImage> deleteImgs = existTour.TourImages.Where(ti => !tourVM.PhotoIds.Exists(imgId => ti.Id == imgId) && ti.IsPrimary == null).ToList();
            deleteImgs.ForEach(di => di.Image.DeleteFile(_env.WebRootPath, "assets", "images", "trending"));
            _context.TourImages.RemoveRange(deleteImgs);
            if (tourVM.Photo != null)
            {
                if (!tourVM.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateTourVM.Photo), "Invalid image type.");
                    return View(tourVM);
                }

                if (!tourVM.Photo.ValidateSize(FileSize.MB, 2))
                {
                    ModelState.AddModelError(nameof(UpdateTourVM.Photo), "Image size cannot exceed 2MB.");
                    return View(tourVM);
                }



            }

            if (tourVM.Photo != null)
            {
                TourImage main = new TourImage
                {
                    Image = await tourVM.Photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                    IsPrimary = true,
                    CreatedAt = DateTime.Now
                };
                TourImage? exitedMain = existTour.TourImages.FirstOrDefault(p => p.IsPrimary == true);
                exitedMain.Image.DeleteFile(_env.WebRootPath, "assets", "images", "trending");
                existTour.TourImages.Remove(exitedMain);
                existTour.TourImages.Add(main);
            }
            if (tourVM.AdditionalPhotos is not null)
            {
                string text = string.Empty;
                foreach (var photo in tourVM.AdditionalPhotos)
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
                    existTour.TourImages.Add(new TourImage
                    {
                        Image = await photo.CreateFileAsync(_env.WebRootPath, "assets", "images", "trending"),
                        IsPrimary = null,
                        CreatedAt = DateTime.Now

                    });
                }
                TempData["FileWarning"] = text;
            }
            existTour.TourTranslations.FirstOrDefault().Title = tourVM.Title;
            existTour.TourTranslations.FirstOrDefault().Description = tourVM.Description;
            existTour.Price = tourVM.Price;
            existTour.Duration = tourVM.Duration;
            existTour.Start_Date = tourVM.Start_Date;
            existTour.End_Date = tourVM.End_Date;
            existTour.Available_seats = tourVM.Available_seats ?? 0;
            existTour.TourTranslations.FirstOrDefault().Location = tourVM.Location;
            existTour.DestinationId = tourVM.DestinationId;
            existTour.UpdateAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Tour? tour = await _context.Tours.Include(t => t.TourImages).FirstOrDefaultAsync(s => s.Id == id);
            if (tour is null) return NotFound();
            foreach (TourImage TImage in tour.TourImages)
            {
                TImage.Image.DeleteFile(_env.WebRootPath, "assets", "images", "website-images");
            };
            _context.Remove(tour);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
