using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.DAL.Models
{
    public class Template : Entity
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public TemplateType TemplateType { get; set; }

        [InverseProperty("Template")]
        public virtual ICollection<Field> Fields { get; set; }

        public virtual ICollection<View> Views { get; set; }
    }
}
