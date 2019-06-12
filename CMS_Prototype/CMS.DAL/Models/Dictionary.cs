using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.DAL.Models
{
    public class Dictionary : Entity
    {
        public string Name { get; set; }

        public DictionaryType DictionaryType { get; set; }

        [InverseProperty("Dictionary")]
        public virtual ICollection<Permission> Permissions { get; set; }
    }
}
