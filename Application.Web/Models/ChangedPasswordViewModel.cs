using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Web.Models
{
    public class ChangedPasswordViewModel
    {
        [Required(ErrorMessage = "Password field is required.")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "New password field is required.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Confirm password field is required.")]
        public string ConfirmPassword { get; set; }
    }
}
