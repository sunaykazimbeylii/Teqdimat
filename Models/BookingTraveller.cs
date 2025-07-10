using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Models.Base;
using TravelFinalProject.Utilities.Enums;

namespace TravelFinalProject.Models
{
    public class BookingTraveller : BaseEntity
    {
        public int BookingId { get; set; }
        public Booking? Booking { get; set; }


        [Required]
        [DataType(DataType.Date)]
        public DateOnly DateOfBirth { get; set; }
        public string PassportNumber { get; set; }

        public ICollection<BookingTravellerTranslation> BookingTravellerTranslations { get; set; }

        public BookingTraveller()
        {
            BookingTravellerTranslations = new HashSet<BookingTravellerTranslation>();
        }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }



    }
    public class BookingTravellerTranslation : BaseEntity
    {
        public string LangCode { get; set; }
        [Required]
        public Gender Gender { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public string Nationality { get; set; }
        public int BookingTravellerId { get; set; }
    }
}

