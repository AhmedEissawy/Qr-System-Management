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

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string VisitorName { get; set; }

        public string VisitorIdentifier { get; set; }

        public string UnitName { get; set; }

        public bool Approve { get; set; }

        public int OwnerId { get; set; }

        public virtual Owner Owner { get; set; }
    }

}