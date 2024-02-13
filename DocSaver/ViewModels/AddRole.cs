using System.ComponentModel.DataAnnotations;

namespace DocSaver.ViewModels
{
    public class AddRole
    {
        [Required(ErrorMessage = "Role Name Required!")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Enter Text Only!")]
        public string RoleName { get; set; }
    }
}
