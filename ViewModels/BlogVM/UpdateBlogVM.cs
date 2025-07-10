using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels.BlogVM
{
    public class UpdateBlogVM
    {
        public string ImageUrl { get; set; }
        public IFormFile? Photo { get; set; }
        public DateTime PublishedDate { get; set; }

        public bool IsPopular { get; set; } = false;
        [Required, StringLength(100)]

        public string Title { get; set; }
        [Required, StringLength(500)]

        public string Content { get; set; }
    }
}
