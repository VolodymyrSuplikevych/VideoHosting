using System.ComponentModel.DataAnnotations;

namespace VideoHosting.Core.Models
{
    public class UserRegistrationModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }
        
        [Required]
        public string Group { get; set; }
    }
}