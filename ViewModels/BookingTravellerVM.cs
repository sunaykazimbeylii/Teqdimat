using System.ComponentModel.DataAnnotations;
using TravelFinalProject.Utilities.Enums;

namespace TravelFinalProject.ViewModels
{
    public class BookingTravellerVM
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string PassportNumber { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public string Nationality { get; set; }

        public DateOnly? DateOfBirth { get; set; }
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string? PhoneNumber { get; set; }
    }

}