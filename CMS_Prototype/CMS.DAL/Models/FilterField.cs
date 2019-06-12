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
    public class FilterField : IFilterField
    {
        [Key, Column(Order = 0)]
        public int FilterId { get; set; }

        [Key, Column(Order = 1)]
        public int FieldId { get; set; }

        [Key, Column(Order = 2)]
        public int ChainId { get; set; }

        public int Depth { get; set; }

        public virtual Filter Filter { get; set; }

        public virtual Field Field { get; set; }

        public Field GetField()
        {
            return Field;
        }

        public IFilter GetFilter()
        {
            return Filter;
        }
    }
}
