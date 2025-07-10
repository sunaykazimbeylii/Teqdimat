using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Models;
using TravelFinalProject.Utilities.Enums;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels;

namespace TravelFinalProject.Controllers
{
    public class TourController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public TourController(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }
        public async Task<IActionResult> Index(
         TourSearchVM search,
         int? destinationId,
         int? min_price,
         int? max_price,
         int page = 1,
         int key = 1

       )
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;


            string currencyCode = Request.Cookies["SelectedCurrency"] ?? "USD";
            int pageSize = 3;


            var query = _context.Tours
                .Include(t => t.TourTranslations)
                .Include(t => t.TourImages.Where(ti => ti.IsPrimary == true))
                .Include(t => t.Destination)
                    .ThenInclude(d => d.DestinationTranslations)
                .AsQueryable();


            query = query.Where(t => t.TourTranslations.Any(tr => tr.LangCode == langCode));


            if (destinationId.HasValue && destinationId > 0)
                query = query.Where(t => t.DestinationId == destinationId);

            if (min_price.HasValue)
                query = query.Where(t => t.Price >= min_price.Value);

            if (max_price.HasValue)
                query = query.Where(t => t.Price <= max_price.Value);

            var today = DateOnly.FromDateTime(DateTime.Now);
            query = query.Where(t => t.End_Date >= today);

            if (!string.IsNullOrEmpty(search.Destination))
            {
                query = query.Where(t =>
                    t.Destination.DestinationTranslations.Any(dt =>
                        dt.LangCode == langCode && dt.Name == search.Destination));
            }


            if (search.CheckIn.HasValue)
            {
                DateTime checkIn = search.CheckIn.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(t => t.Start_Date.ToDateTime(TimeOnly.MinValue) >= checkIn);
            }

            if (search.CheckOut.HasValue)
            {
                DateTime checkOut = search.CheckOut.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(t => t.End_Date.ToDateTime(TimeOnly.MaxValue) <= checkOut);
            }


            if (search.Adults.HasValue)
                query = query.Where(t => t.Available_seats >= search.Adults);


            switch (key)
            {
                case (int)SortType.Price:
                    query = query.OrderBy(t => t.Price);
                    break;
                case (int)SortType.Date:
                    query = query.OrderByDescending(t => t.CreatedAt);
                    break;
                case (int)SortType.Duration:
                    query = query.OrderByDescending(t => t.Duration);
                    break;
                default:
                    query = query.OrderBy(t => t.Id);
                    break;
            }


            int count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1 || page > totalPages) throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");

            var pagedTours = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            var tourVMs = new List<GetTourVM>();
            foreach (var t in pagedTours)
            {
                var translation = t.TourTranslations.FirstOrDefault(tr => tr.LangCode == langCode);
                var destinationTranslation = t.Destination?.DestinationTranslations.FirstOrDefault(dt => dt.LangCode == langCode);

                decimal originalPrice = t.Price ?? 0;
                decimal convertedPrice = await _currencyService.ConvertAsync(originalPrice, currencyCode);

                tourVMs.Add(new GetTourVM
                {
                    Id = t.Id,
                    Title = translation?.Title ?? "No title",
                    DestinationId = t.DestinationId,
                    Start_Date = t.Start_Date,
                    End_Date = t.End_Date,
                    Location = translation?.Location,
                    Available_seats = t.Available_seats,
                    Image = t.TourImages.FirstOrDefault()?.Image ?? "default.jpg",
                    Description = translation?.Description,
                    DestinationName = destinationTranslation?.Name,
                    Price = convertedPrice,
                    CurrencyCode = currencyCode
                });
            }

            var paginatedVM = new PaginatedVM<GetTourVM>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = tourVMs
            };


            var destinations = await _context.Destinations
                .Include(d => d.DestinationTranslations.Where(dt => dt.LangCode == langCode))
                .ToListAsync();

            var vm = new TourListPageVM
            {
                PaginatedTours = paginatedVM,
                Destinations = destinations,
                SearchForm = new TourSearchVM()
            };

            return View(vm);
        }






        public async Task<IActionResult> TourDetail(int? id)
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            if (id is null || id < 1) throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");

            Tour tour = await _context.Tours
                .Include(t => t.TourTranslations.Where(tt => tt.LangCode == langCode))
                .Include(t => t.Destination).ThenInclude(d => d.DestinationTranslations)
                .Include(t => t.TourImages)
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tour == null) throw new NotFoundException("tapilmadi");


            var reviews = await _context.Reviews
                .Where(r => r.TourId == tour.Id && r.IsApproved)
                .Include(r => r.ReviewTranslations)
                .Include(r => r.User)
                .ToListAsync();

            decimal originalPrice = tour.Price ?? 0m;
            string currencyCode = Request.Cookies["currency"] ?? "USD";
            decimal convertedPrice = await _currencyService.ConvertAsync(originalPrice, currencyCode);
            string currencySymbol = _currencyService.GetSymbol(currencyCode);

            TourDetailVM detailVM = new TourDetailVM
            {
                Tour = tour,
                ConvertedPrice = convertedPrice,
                CurrencySymbol = currencySymbol,
                RelatedTour = await _context.Tours
                    .Include(t => t.TourTranslations.Where(tt => tt.LangCode == langCode))
                    .Where(t => t.DestinationId == tour.DestinationId && t.Id != id)
                    .Include(t => t.TourImages)
                    .ToListAsync(),
                Reviews = reviews
            };

            return View(detailVM);
        }

    }
}
