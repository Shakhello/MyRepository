using CMS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Interfaces
{
    public interface IFilterField
    {
        int ChainId { get; set; }

        int Depth { get; set; }

        IFilter GetFilter();

        Field GetField();
    }
}
