using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class InvitationDto
    {
        public DateTime startDate { get; set; }

        public DateTime endDate { get; set; }

        public string visitorName { get; set; }

        public string sSN { get; set; }

        public string unitName { get; set; }

        public string ownerName { get; set; }

        public bool Approve { get; set; }

        public string message { get; set; }

        public bool isSuccess { get; set; }
    }
}