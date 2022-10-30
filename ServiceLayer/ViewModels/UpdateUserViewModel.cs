using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class UpdateUserViewModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="Password and ConfirmPassword does not match")]
        public string ConfirmPassword { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}