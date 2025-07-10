using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class DestinationImage : BaseEntity
    {
        public string Image { get; set; }
        public bool? IsPrimary { get; set; }
        public int DestinationId { get; set; }
        public Destination? Destination { get; set; }
    }
}
