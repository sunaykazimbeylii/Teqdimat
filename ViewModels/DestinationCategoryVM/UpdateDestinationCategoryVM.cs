using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels
{
    public class UpdateDestinationCategoryVM
    {
        [StringLength(50)]
        public string Name { get; set; }
    }
}
