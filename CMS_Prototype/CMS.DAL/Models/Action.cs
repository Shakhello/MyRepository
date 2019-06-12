using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.DAL.Models
{
    public class Action : Entity
    {
        public int EventId { get; set; }

        public int Order { get; set; }

        public ActionType ActionType { get; set; }

        public virtual Event Event { get; set; }

        [InverseProperty("Action")]
        public virtual ICollection<Parameter> Parameters { get; set; }
    }
}
