using System;
using System.ComponentModel.DataAnnotations;

namespace BEonTime.Data.Models
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
        [Required(ErrorMessage = "Department name cannot be empty!")]
        public string DepartmentName { get; set; }
        [Required(ErrorMessage = "Career started date cannot be empty!")]
        public DateTime CareerStarted { get; set; }
    }
}
