using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CMS.DAL.Models
{
    public class Control : Entity
    {
        public int? StyleId { get; set; }

        public int ViewId { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public bool ShowDisplayName { get; set; }

        public int OrderIndex { get; set; }

        public ControlType ControlType { get; set; }

        public string DefaultValue { get; set; }

        public bool Required { get; set; }

        public string Pattern { get; set; }

        public int GridIndex { get; set; }

        public int? MaxLength { get; set; }

        public int? Width { get; set; }

        public virtual Style Style { get; set; }

        [Required]
        public virtual View View { get; set; }

        public virtual ICollection<ControlField> ControlFields { get; set; }

        public virtual ICollection<Parameter> Parameters { get; set; }

        public virtual ICollection<Event> Events { get; set; }

        [NotMapped]
        public int? FieldId
        {
            get => Field?.Id;
        }

        [NotMapped]
        public virtual Field Field
        {
            get => ControlFields.OrderBy(cf => cf.Depth).Select(cf => cf.Field).LastOrDefault();
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Control))
                return false;

            return Id == ((Control)other).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();

        }
    }
}
