using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels.TourVM
{
    public class CreateTourVM
    {
        public string LangCode { get; set; }
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
        public IFormFile Photo { get; set; }

        public List<IFormFile>? AdditionalPhotos { get; set; }

        [Required]
        public int? DestinationId { get; set; }
        public List<Destination>? Destinations { get; set; }
        public List<DestinationTranslation>? DestinationTranslations { get; set; }

    }
}
