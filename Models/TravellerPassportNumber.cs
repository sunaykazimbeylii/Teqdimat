using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class TravellerPassportNumber : BaseEntity
    {
        public int BookingTravellerId { get; set; }
        public BookingTraveller BookingTraveller { get; set; }
        public int PassportNumber { get; set; }
    }
}
