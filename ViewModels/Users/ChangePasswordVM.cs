using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels.Users
{
    public class ChangePasswordVM
    {

        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }


    }
}
