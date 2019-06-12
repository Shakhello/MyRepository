using CMS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class VirtualFilterField : IFilterField
    {
        public int ChainId { get; set; }

        public int Depth { get; set; }

        public IFilter Filter { get; set; }

        public Field Field { get; set; }

        public IFilter GetFilter()
        {
            return Filter;
        }

        public Field GetField()
        {
            return Field;
        }
    }
}
