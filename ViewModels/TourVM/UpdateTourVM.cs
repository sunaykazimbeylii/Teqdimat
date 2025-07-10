using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels.TourVM
{
    public class UpdateTourVM
    {
        [Required, StringLength(200)]
        public string Title { get; set; }
        [StringLength(2000)]
        public string Description { get; set; }
        [Required]
        public decimal? Price { get; set; }
        public string Duration { get; set; }
        [Required]
        public DateOnly Start_Date { get; set; }
        [Required]
        public DateOnly End_Date { get; set; }
        [Range(1, int.MaxValue)]
        [Required]
        public int? Available_seats { get; set; }
        public string Location { get; set; }
        public List<TourImage>? TourImages { get; set; }
        public string? Image { get; set; }
        public IFormFile? Photo { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }
        public List<int>? PhotoIds { get; set; }
        [Required]
        public int? DestinationId { get; set; }
        public List<Destination>? Destinations { get; set; }
        public List<DestinationTranslation>? DestinationTranslations { get; set; }


        public DateTime UpdateAt { get; set; }
    }
}
