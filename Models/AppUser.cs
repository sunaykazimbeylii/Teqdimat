using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.Models
{
    public class AppUser : IdentityUser
    {
        public string? Image { get; set; }
        public string? Bio { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? DateOfBirth { get; set; }
    }
}
