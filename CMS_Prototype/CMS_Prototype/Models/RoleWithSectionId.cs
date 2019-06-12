using CMS.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unity.Models
{
    public class RoleWithSectionId
    {
        public RoleDefinition Role { get; set; }

        public int SectionId { get; set; }
    }
}