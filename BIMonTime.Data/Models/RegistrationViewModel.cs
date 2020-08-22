using System.ComponentModel.DataAnnotations;

namespace BIMonTime.Data.Models
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = "Email cannot be empty!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password cannot be empty!")]
        public string Password { get; set; }
        [Required(ErrorMessage = "First name cannot be empty!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name cannot be empty!")]
        public string LastName { get; set; }
    }
}
