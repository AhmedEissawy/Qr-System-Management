using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class OwnersCount
    {
        public int PendingCount { get; set; }

        public int ApprovedCount { get; set; }

        public int RejectedOwners { get; set; }
    }
}