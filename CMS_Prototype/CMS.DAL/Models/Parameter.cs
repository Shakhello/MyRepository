using System.ComponentModel.DataAnnotations;

namespace CMS.DAL.Models
{
    public class Parameter : Entity
    {
        public ParameterType ParameterType { get; set; }

        public string Name { get; set; }

        public int? ControlId { get; set; }

        public int ActionId { get; set; }

        public int? FieldId { get; set; }

        public int Order { get; set; }

        [Required]
        public virtual Action Action { get; set; }

        public virtual Field Field { get; set; }

        public virtual Control Control { get; set; }

        public string DefaultValue { get; set; }
    }
}
