using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DocSaver.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "Username Required!")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email Required!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Cofirm Password!")]
        [Compare("Password", ErrorMessage = "Password and Confirm Password not matched!")]
        public string ConfirmPassword { get; set; } = default!;
        public string Role { get; set; }
    }
}
