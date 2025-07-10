using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels
{
    public class CreateDestinationCategoryVM
    {
        public string LangCode { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
