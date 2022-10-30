using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [StringLength(150)]
        [Display(Name ="Email Or UserName")]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string Password { get; set; }
    }

}
