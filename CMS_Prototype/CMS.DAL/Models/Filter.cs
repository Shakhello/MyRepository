using CMS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class Filter : Entity, IFilter
    {
        public int ViewId { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        public OperationType? Operation { get; set; }

        public FilterType? FilterType { get; set; }

        public string DefaultValue { get; set; }

        public int? Order { get; set; }

        public int? Width { get; set; }
        
        [Required]
        public virtual View View { get; set; }

        public virtual ICollection<FilterField> FilterFields { get; set; }

        public List<IFilterField> GetFilterFields()
        {
            return FilterFields.Select(ff => (IFilterField)ff).ToList();
        }

        [NotMapped]
        public virtual List<Field> Fields
        {
            get => FilterFields.Select(ff => ff.Field).Where(f => f.FieldType != FieldType.Reference).ToList();
        }

        public override bool Equals(object other)
        {
            if (other == null || !(other is Filter))
                return false;

            return Id == ((Filter)other).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();

        }

    }
}
