
using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{

    public class BookingDetailVM
    {
        public Booking Booking { get; set; }
        public List<BookingTraveller> Travellers { get; set; }

    }
}
