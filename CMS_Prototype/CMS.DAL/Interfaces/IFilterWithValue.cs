using CMS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interfaces
{
    public interface IFilterWithValue
    {
        IFilter GetFilter();

        object GetValue();
    }
}
