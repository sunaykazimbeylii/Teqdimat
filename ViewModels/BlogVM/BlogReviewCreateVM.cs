using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels.BlogVM
{
    public class BlogReviewCreateVM
    {
        [Required]
        public int BlogId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
