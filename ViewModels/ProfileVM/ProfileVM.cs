using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using TravelFinalProject.ViewModels.Users;

namespace TravelFinalProject.ViewModels.ProfileVM
{
    public class ProfileVM
    {
        public IFormFile? Photo { get; set; }
        public string? Image { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        [ValidateNever]
        public ChangePasswordVM ChangePassword { get; set; } = new();
    }
}
