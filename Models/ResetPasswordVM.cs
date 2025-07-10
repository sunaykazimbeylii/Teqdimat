using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.Models
{
    public class ResetPasswordVM
    {
        [Required]
        public string Token { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required, DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
