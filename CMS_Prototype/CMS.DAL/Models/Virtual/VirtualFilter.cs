using CMS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class VirtualFilter : IFilter
    {
        public int ViewId { get; set; }

        public OperationType? Operation { get; set; }

        public List<IFilterField> FilterFields { get; set; }

        public List<IFilterField> GetFilterFields()
        {
            return FilterFields;
        }

        public virtual List<Field> Fields
        {
            get => FilterFields.Select(ff => ff.GetField()).Where(f => f.FieldType != FieldType.Reference).ToList();
        }
    }
}
