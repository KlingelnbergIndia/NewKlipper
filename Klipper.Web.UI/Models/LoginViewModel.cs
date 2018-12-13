using System.ComponentModel.DataAnnotations;

namespace Klipper.Web.UI.Models
{
    public class LoginViewModel
    {
        [Required]
        [MinLength(8)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
