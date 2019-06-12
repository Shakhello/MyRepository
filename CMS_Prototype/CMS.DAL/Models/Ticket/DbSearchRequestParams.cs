using CMS.DAL.Common;
using System.Linq;

namespace CMS.DAL.Models
{
    public class DbSearchRequestParams
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public Field SortField { get; set; }

        public SortDirection SortDirection { get; set; }

        public bool SearchWithInnerJoin { get; set; }

        public DbSearchRequestParams(int pageNumber, int pageSize, Field sortField, string sortDirection)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            SortField = sortField;
            SortDirection = (sortDirection.ToLower() == "asc") ? SortDirection.Asc : SortDirection.Desc;
        }

        public string ConvertToString(string sortFieldTableAlias)
        {
            var sortDirection = SortDirection == SortDirection.Asc ? "asc" : "desc";

            var sort = string.IsNullOrEmpty(sortFieldTableAlias) ? $"(select null)" : $"{sortFieldTableAlias}.{SortField.Name} {sortDirection}";

            return $"order by {sort} offset { (PageNumber - 1) * PageSize } rows fetch next {PageSize} rows only";
        }

        public static DbSearchRequestParams GetDefault(View view)
        {
            Field field = view.Controls.First().Field;
            return new DbSearchRequestParams(1, Constants.TABLE_PAGE_SIZE, field, "asc");
        }
    }
}
