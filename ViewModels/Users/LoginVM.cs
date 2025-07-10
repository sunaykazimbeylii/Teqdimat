using System.ComponentModel.DataAnnotations;

namespace TravelFinalProject.ViewModels.Users
{
    public class LoginVM
    {
        [MaxLength(150)]
        public string UserNameOrEmail { get; set; }
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistent { get; set; }
    }
}
