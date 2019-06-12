using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class Role : Entity
    {
        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string DisplayName { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        [InverseProperty("Role")]
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
