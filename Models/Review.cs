using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class Review : BaseEntity
    {
        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        public string UserId { get; set; }
        public AppUser? User { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public bool IsApproved { get; set; } =
            false;
        public string LangCode { get; set; } = "az";
        public ICollection<ReviewTranslation> ReviewTranslations { get; set; }

        public Review()
        {
            ReviewTranslations = new HashSet<ReviewTranslation>();
        }
    }
    public class ReviewTranslation : BaseEntity
    {
        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }

        public int ReviewId { get; set; }
    }
}
