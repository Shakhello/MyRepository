using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.UI
{
    public class UserDefinition : Definition
    {
        public string Login { get; set; }

        public List<UserRoleDefinition> UserRoles { get; set; } = new List<UserRoleDefinition>();

    }
}
