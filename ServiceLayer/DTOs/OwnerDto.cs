using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class OwnerDto
    {
        public int id { get; set; }

        public string ownerName { get; set; }

        public string Image { get; set; }

        public string ownerUnit { get; set; }

        public string ownerPhone { get; set; }

        public string ownerEmail { get; set; }

        public bool Switch { get; set; }
    }
}