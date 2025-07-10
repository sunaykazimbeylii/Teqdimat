using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;

namespace TravelFinalProject.Models
{
    public class BlogReviewReply : BaseEntity
    {
        public int BlogReviewId { get; set; }
        public BlogReview BlogReview { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Comment { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
