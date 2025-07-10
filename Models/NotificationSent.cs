using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class NotificationSent : BaseEntity
    {
        public string UserId { get; set; }
        public int TourId { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsReviewGiven { get; set; } = false;

        // Navigation properties, məsələn:
        public AppUser User { get; set; }
        public Tour Tour { get; set; }
    }
}
