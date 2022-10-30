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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Email { get; set; }

        public bool View { get; set; } = true;

        public bool Switch { get; set; }

        public string Type { get; set; }

        public string Image { get; set; }

        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual Unit Unit { get; set; }

        public virtual ICollection<Invitaion> Invitaions { get; set; }

    }
}