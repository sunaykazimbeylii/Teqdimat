using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class Slide : BaseEntity
    {
        [StringLength(300)]
        public string ButtonUrl { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public ICollection<SlideTranslation> SlideTranslations { get; set; }

        public Slide()
        {
            SlideTranslations = new HashSet<SlideTranslation>();
        }

    }
    public class SlideTranslation : BaseEntity
    {
        public string LangCode { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; }

        [StringLength(300)]
        public string Subtitle { get; set; }

        [StringLength(100)]
        public string ButtonText { get; set; }
        public int SlideId { get; set; }
    }

}
