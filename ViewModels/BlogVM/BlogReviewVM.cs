namespace TravelFinalProject.ViewModels.BlogVM
{
    public class BlogReviewVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string? UserImage { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<BlogReviewReplyVM>? Replies { get; set; }
    }
}
