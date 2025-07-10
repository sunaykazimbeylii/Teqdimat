using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class HomeVM
    {
        public List<Tour> Tours { get; set; }
        public List<Destination> Destinations { get; set; }
        public List<Slide> Slides { get; set; }
        public List<DestinationCategory> DestinationCategories { get; set; }
        public List<DestinationImage> DestinationImages { get; set; }
        public List<TourImage> TourImages { get; set; }
        public List<Review> Reviews { get; set; }
        public int? CurrentCategoryId { get; set; }
        public TourSearchVM Search { get; set; }

    }
}
