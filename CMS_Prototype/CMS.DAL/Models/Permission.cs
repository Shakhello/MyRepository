using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class Permission : Entity
    {
        public int RoleId { get; set; }

        public int? ViewId { get; set; }

        public PermissionType PermissionType { get; set; }

        public virtual View View { get; set; }

        public virtual Dictionary Dictionary { get; set; }

        public virtual Role Role { get; set; }
    }
}
