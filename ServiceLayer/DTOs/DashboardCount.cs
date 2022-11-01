using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class DashboardCount
    {
        public int userCount { get; set; }

        public int ownerCount { get; set; }

        public int invitationCount { get; set; }

        public int unitCount { get; set; }
    }
}