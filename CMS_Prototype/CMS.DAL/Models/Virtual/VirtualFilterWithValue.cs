using CMS.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class VirtualFilterWithValue : IFilterWithValue
    {
        public IFilter Filter { get; set; }

        public object Value { get; set; }

        public IFilter GetFilter()
        {
            return Filter;
        }

        public object GetValue()
        {
            return Value;
        }
    }
}
