using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class DestinationListVM
    {
        public PaginatedVM<GetDestinationVM> PaginatedDestinations { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }

        public List<DestinationCategory> Categories { get; set; }
        public int? CurrentCategoryId { get; set; }
    }
}
