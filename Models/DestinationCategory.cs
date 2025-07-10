using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class DestinationCategory : BaseEntity
    {

        public ICollection<DestinationCategoryTranslation> DestinationCategoryTranslations { get; set; }

        public DestinationCategory()
        {
            DestinationCategoryTranslations = new HashSet<DestinationCategoryTranslation>();
        }
        public List<Destination>? Destinations { get; set; }
    }
    public class DestinationCategoryTranslation : BaseEntity
    {
        public string LangCode { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int DestinationCategoryId { get; set; }
    }
}
