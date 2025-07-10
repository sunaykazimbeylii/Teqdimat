using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TravelFinalProject.DAL;
using TravelFinalProject.Interfaces;
using TravelFinalProject.Utilities.Exceptions;
using TravelFinalProject.ViewModels;

namespace TravelFinalProject.Controllers
{
    public class DestinationController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ICurrencyService _currencyService;

        public DestinationController(AppDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }
        public async Task<IActionResult> Index(
     int? categoryId,
     int? min_price,
     int? max_price,
     int page = 1
     )
        {
            var langCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

            string currencyCode = Request.Cookies["SelectedCurrency"] ?? "USD";
            int pageSize = 3;

            var query = _context.Destinations
                .Include(d => d.DestinationTranslations)
                .Include(d => d.Category)
                .Include(d => d.DestinationImages.Where(di => di.IsPrimary == true))
                .AsQueryable();

            query = query.Where(d => d.DestinationTranslations.Any(dt => dt.LangCode == langCode));

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(d => d.CategoryId == categoryId);

            if (min_price.HasValue)
                query = query.Where(d => d.Price >= min_price.Value);

            if (max_price.HasValue)
                query = query.Where(d => d.Price <= max_price.Value);

            int count = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (page < 1 || page > totalPages) throw new BadRequestException("Səhv sorğu: Yanlış və ya boş id göndərildi.");

            var pagedDestinations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var destinationVMs = new List<GetDestinationVM>();
            foreach (var d in pagedDestinations)
            {
                var translation = d.DestinationTranslations.FirstOrDefault(dt => dt.LangCode == langCode);
                decimal originalPrice = d.Price ?? 0;
                decimal convertedPrice = await _currencyService.ConvertAsync(originalPrice, currencyCode);

                destinationVMs.Add(new GetDestinationVM
                {
                    Id = d.Id,
                    Name = translation?.Name,
                    Description = translation?.Description,
                    City = translation.City,
                    Country = translation.Country,
                    Address = translation.Address,
                    CategoryName = translation.Name,
                    MainImage = d.DestinationImages.FirstOrDefault(di => di.IsPrimary == true).Image,
                    Price = convertedPrice,
                    CurrencyCode = currencyCode
                });
            }

            var paginatedVM = new PaginatedVM<GetDestinationVM>
            {
                TotalPage = totalPages,
                CurrentPage = page,
                Items = destinationVMs
            };

            var categories = await _context.DestinationCategories
                .Include(c => c.DestinationCategoryTranslations.Where(ct => ct.LangCode == langCode))
                .ToListAsync();

            var vm = new DestinationListVM
            {
                PaginatedDestinations = paginatedVM,
                Categories = categories,
                CurrentCategoryId = categoryId
            };

            return View(vm);
        }

    }
}
