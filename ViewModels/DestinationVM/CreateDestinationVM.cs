using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class CreateDestinationVM
    {
        public string LangCode { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        public string Country { get; set; }
        public IFormFile MainPhoto { get; set; }
        public string City { get; set; }
        [Required]
        public decimal? Price { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
        [Required]
        public int? CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }
        public List<DestinationCategory>? Categories { get; set; }
        public List<DestinationCategoryTranslation>? DestinationCategories { get; set; }


    }
}
