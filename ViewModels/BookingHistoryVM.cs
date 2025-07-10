using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class BookingHistoryVM
    {
        public List<Booking> Bookings { get; set; }
        public AppUser User { get; set; } = new AppUser();
        public List<BookingTraveller> BookingTravellers { get; set; }
    }
}
