using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Unit
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public int OwnerId { get; set; }

        public virtual Owner Owner { get; set; }
    }

}

