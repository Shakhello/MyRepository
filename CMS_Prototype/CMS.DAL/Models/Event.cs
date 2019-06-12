using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.DAL.Models
{
    public class Event : Entity
    {
        public int ControlId { get; set; }

        public EventType EventType { get; set; }

        [Required]
        public virtual Control Control { get; set; }

        [InverseProperty("Event")]
        public virtual ICollection<Action> Actions { get; set; }


    }
}
