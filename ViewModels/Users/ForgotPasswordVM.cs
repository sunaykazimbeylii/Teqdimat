using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels.Users
{
    public class ForgotPasswordVM
    {
        [Required, EmailAddress, Display(Name = "Registered email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}
