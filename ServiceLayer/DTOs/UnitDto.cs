using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class UnitDto
    {
        public int? id { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        public string Owner { get; set; }

        public string message { get; set; }

        public bool isSuccess { get; set; }
    }
}