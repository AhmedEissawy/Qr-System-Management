using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ViewModels
{
    public class InvitationViewModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string VisitorName { get; set; }

        [Required]
        public string SSN { get; set; }

        public int UnitId { get; set; }

        [Required]
        public string OwnerName { get; set; }
    }
}
