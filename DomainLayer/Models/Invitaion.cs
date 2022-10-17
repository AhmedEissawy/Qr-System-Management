using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Invitaion
    {

        public int Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public string VisitorName { get; set; }

        public string VisitorSSN { get; set; }

        public string UnitName { get; set; }

        public int OwnerId { get; set; }

        public Owner Owner { get; set; }
    }

}

