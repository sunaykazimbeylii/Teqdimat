using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels
{
    public class GetDestinationVM
    {
        public int Id { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(1000)]
        public string Description { get; set; }
        public string Country { get; set; }
        public string MainImage { get; set; }
        public string City { get; set; }
        public string CurrencyCode { get; set; }
        [Required]
        public decimal? Price { get; set; }

        [StringLength(200)]
        public string Address { get; set; }
        public bool IsFeatured { get; set; }
        public string CategoryName { get; set; }

    }
}
