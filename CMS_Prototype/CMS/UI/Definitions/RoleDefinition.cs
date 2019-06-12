using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class RoleDefinition : Definition
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool CanBeEditedByCurrentUser { get; set; }

        public List<UserDefinition> Users { get; set; } = new List<UserDefinition>();

        public List<PermissionDefinition> Permissions { get; set; } = new List<PermissionDefinition>();
    }
}
