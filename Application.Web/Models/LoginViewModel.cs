using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Web.Models
{
    public class LoginViewModel : LayoutViewModel
    {
        [Required(ErrorMessage = "User name field is required.")]
        [MinLength(5)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password field is required.")]
        public string Password { get; set; }
    }
}
