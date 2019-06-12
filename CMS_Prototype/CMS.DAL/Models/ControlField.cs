using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class ControlField
    {
        [Key, Column(Order = 0)]
        public int ControlId { get; set; }

        [Key, Column(Order = 1)]
        public int FieldId { get; set; }

        public int Depth { get; set; }

        public virtual Control Control { get; set; }

        public virtual Field Field { get; set; }
    }
}
