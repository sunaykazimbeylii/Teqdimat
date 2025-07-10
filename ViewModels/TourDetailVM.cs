using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class TourDetailVM
    {
        public int Id { get; set; }
        public decimal ConvertedPrice { get; set; }
        public string CurrencySymbol { get; set; }
        public Tour Tour { get; set; }
        public List<Tour> RelatedTour { get; set; }
        public TourTranslation SelectedTranslation { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
