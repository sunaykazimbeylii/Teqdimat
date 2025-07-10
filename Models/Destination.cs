using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class Destination : BaseEntity
    {
        [Required]
        public int? CategoryId { get; set; }
        public bool IsFeatured { get; set; }
        [Required]
        public decimal? Price { get; set; }

        public List<DestinationImage>? DestinationImages { get; set; }
        public ICollection<DestinationTranslation> DestinationTranslations { get; set; }

        public Destination()
        {
            DestinationTranslations = new HashSet<DestinationTranslation>();
        }
        public DestinationCategory? Category { get; set; }
        public List<Tour>? Tours { get; set; }
    }
    public class DestinationTranslation : BaseEntity
    {
        public string LangCode { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string Country { get; set; }
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
        public int DestinationId { get; set; }
    }
}
