using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class Blog : BaseEntity
    {
        public string ImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsPopular { get; set; } = false;
        public ICollection<BlogTranslation> BlogTranslations { get; set; }
        public List<BlogReview>? Reviews { get; set; } = new List<BlogReview>();

        public Blog()
        {
            BlogTranslations = new HashSet<BlogTranslation>();
        }
    }
    public class BlogTranslation : BaseEntity
    {
        public string LangCode { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; }
        [Required, StringLength(500)]
        public string Content { get; set; }
        public int BlogId { get; set; }
    }
}
