using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class UserRoleDefinition : Definition
    {
        public int UserId { get; set; }

        public int RoleId { get; set; }

        public bool UserCanChangeRole { get; set; }
    }
}
