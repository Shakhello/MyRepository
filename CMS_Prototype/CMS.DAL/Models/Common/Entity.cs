using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public abstract class Entity
    {
        public int Id { get; set; }

        //public abstract override bool Equals(object other);

        //public abstract override int GetHashCode();
    }
}
