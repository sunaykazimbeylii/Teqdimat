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
    public class BookingController : Controller
    {
        private readonly AppDbContext _context;

        public BookingController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var bookings = _context.Bookings
                   .Include(b => b.Tour).ThenInclude(b => b.TourTranslations)
                   .Include(b => b.User)
                   .ToList();
            var travellers = _context.BookingTravellers
                .Include(bt => bt.BookingTravellerTranslations)
                .Where(t => bookings.Select(b => b.Id).Contains(t.BookingId))
                .ToList();


            var model = new BookingHistoryVM
            {
                Bookings = bookings,
                BookingTravellers = travellers ?? new List<BookingTraveller>(),
                User = bookings.FirstOrDefault()?.User ?? new AppUser()
            };

            return View(model);
        }
    }
}
