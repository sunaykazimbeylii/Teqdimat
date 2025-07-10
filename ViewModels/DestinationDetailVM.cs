using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class DestinationDetailVM
    {
        public Destination Destination { get; set; }
        public List<Destination> RelatedDestinations { get; set; }
    }
}
