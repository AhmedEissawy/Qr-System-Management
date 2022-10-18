using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Owner
    {

        public Owner()
        {
            Invitaions = new HashSet<Invitaion>();
        }

        public int Id { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public Unit Unit { get; set; }

        public virtual ICollection<Invitaion> Invitaions { get; set; }

    }
}

