using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(256)]
        [Compare("NewPassword")]
        public string ConfirmNewPassword { get; set; }
    }
}
