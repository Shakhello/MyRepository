using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.DAL.Models
{
    public class Section : Entity
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public virtual ICollection<View> Views { get; set; }
    }
}
