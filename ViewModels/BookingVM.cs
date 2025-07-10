using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models;

namespace TravelFinalProject.ViewModels
{
    public class BookingVM
    {

        public List<BookingTravellerVM> Guests { get; set; } = new();

        public int AdultsCount { get; set; }
        public int ChildrenCount { get; set; }
        public decimal PricePerAdult { get; set; }
        public decimal PricePerChild { get; set; }
        public decimal PromoDiscountPercent { get; set; }
        [Range(1, 100, ErrorMessage = "Qonaq sayı 1 ilə 100 arasında olmalıdır.")]
        public int GuestsCount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public decimal Subtotal => (AdultsCount * PricePerAdult) + (ChildrenCount * PricePerChild);
        public decimal DiscountAmount => PromoDiscountPercent > 0 ? Subtotal * PromoDiscountPercent / 100 : 0;
        public decimal Total => Subtotal - DiscountAmount;

        public int TourId { get; set; }
        public Booking? Booking { get; set; }
        public List<BookingTraveller>? Travellers { get; set; }
    }

}
