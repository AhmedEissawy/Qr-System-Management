using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class ApproveInvitationViewModel
    {

        public int Id { get; set; }

        [Required]
        public bool approve { get; set; }
    }
}