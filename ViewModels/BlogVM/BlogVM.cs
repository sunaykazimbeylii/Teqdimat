namespace TravelFinalProject.ViewModels.BlogVM
{
    public class BlogVM
    {
        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public DateTime PublishedDate { get; set; }

        public bool IsPopular { get; set; } = false;
        public string Title { get; set; }

        public string Content { get; set; }
    }
}
