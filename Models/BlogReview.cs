using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class BlogReview : BaseEntity
    {
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
        public int Rating { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public string Comment { get; set; }
        public List<BlogReviewReply>? Replies { get; set; } = new List<BlogReviewReply>();
    }
}
