using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class TourSearchVM
    {
        public string Destination { get; set; }
        public DateOnly? CheckIn { get; set; }
        public DateOnly? CheckOut { get; set; }
        public int? Adults { get; set; }
        public int? Children { get; set; }
        public int? Duration { get; set; }
        public List<Tour> Results { get; set; }
    }
}
