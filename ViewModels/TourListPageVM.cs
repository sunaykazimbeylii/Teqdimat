using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class TourListPageVM
    {
        public PaginatedVM<GetTourVM> PaginatedTours { get; set; }
        public List<Destination> Destinations { get; set; }
        public TourSearchVM SearchForm { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
