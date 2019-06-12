using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class User : Entity
    {
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string Login { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
