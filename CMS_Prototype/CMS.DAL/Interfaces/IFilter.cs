using CMS.DAL.Models;
using System.Collections.Generic;

namespace CMS.DAL.Interfaces
{
    public interface IFilter
    {
        int ViewId { get; set; }

        OperationType? Operation { get; set; }

        List<IFilterField> GetFilterFields();

        List<Field> Fields { get; }
    }
}
