using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class TourImage : BaseEntity
    {
        public string Image { get; set; }
        public bool? IsPrimary { get; set; }
        public int TourId { get; set; }
        public Tour? Tour { get; set; }
    }
}
