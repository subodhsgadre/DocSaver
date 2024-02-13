using System.ComponentModel.DataAnnotations;

namespace DocSaver.ViewModels
{
    public class Login
    {

        [Required(ErrorMessage = "Username or Email Required!")]
        [Display(Name = "Username/Email")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
