using System.ComponentModel.DataAnnotations;

namespace BEonTime.Data.Models
{
    public class CredentialsViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
