using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Web.Models
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
