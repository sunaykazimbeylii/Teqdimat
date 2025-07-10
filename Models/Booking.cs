using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;
using TravelFinalProject.Utilities;

namespace TravelFinalProject.Models
{
    public class Booking : BaseEntity

    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int TourId { get; set; }
        [Required]
        public DateTime BookingDate { get; set; }
        [Range(1, 100)]
        public int GuestsCount { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerAdult { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PricePerChild { get; set; }

        public decimal Subtotal => (AdultsCount * PricePerAdult) + (ChildrenCount * PricePerChild);
        public int AdultsCount { get; set; }

        public int ChildrenCount { get; set; }
        public int CalculatedGuestsCount => AdultsCount + ChildrenCount;
        public string? PromoCode { get; set; }

        [Range(0, 100)]
        public decimal PromoDiscountPercent { get; set; } = 0;

        public decimal DiscountAmount => PromoDiscountPercent > 0 ? Subtotal * PromoDiscountPercent / 100 : 0;
        public string CurrencyCode { get; set; } = "USD";
        public decimal FinalTotal => Subtotal - DiscountAmount;
        public Tour? Tour { get; set; }
        public AppUser? User { get; set; }
        public List<BookingTraveller>? Travellers { get; set; }

    }
}
