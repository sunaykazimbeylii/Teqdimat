namespace TravelFinalProject.ViewModels.BlogVM
{
    public class BlogDetailVM
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }
        public string UserName { get; set; }

        public DateTime PublishedDate { get; set; }

        public bool IsPopular { get; set; } = false;
        public string Title { get; set; }

        public string Comment { get; set; }
        public int Rating { get; set; }

        public List<BlogReviewVM>? Reviews { get; set; }
    }
}
