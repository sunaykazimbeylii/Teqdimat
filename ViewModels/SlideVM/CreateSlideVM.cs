using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels
{
    public class CreateSlideVM
    {
        public string LangCode { get; set; }
        [Required, StringLength(100)]
        public string Title { get; set; }

        [StringLength(300)]
        public string Subtitle { get; set; }

        [StringLength(100)]
        public string ButtonText { get; set; }

        [StringLength(300)]
        public string ButtonUrl { get; set; }
        public IFormFile Photo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Order 1den az ola bilmez")]
        public int Order { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
    }
}
