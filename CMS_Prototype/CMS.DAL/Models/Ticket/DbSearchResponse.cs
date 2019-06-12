using CMS.DAL.Common;
using CMS.DAL.Interfaces;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class DbSearchResponse
    {
        public DbSearchRequest Request { get; set; }

        public Field SortField { get; private set; }

        public string SortDirection { get; private set; }

        public int PageNumber { get; private set; }

        public int PageSize { get; private set; }

        public int NumberOfPages { get => (int)Math.Ceiling((decimal)Total / PageSize); }

        public int Total { get; private set; }

        public List<DbSearchRecord> Tickets { get; } = new List<DbSearchRecord>();

        public DbSearchResponse(SqlDataReader reader, DbSearchRequest searchRequest)
        {
            Request = searchRequest;

            PageNumber = searchRequest.SortingAndPagingParams.PageNumber;
            PageSize = searchRequest.SortingAndPagingParams.PageSize;
            SortField = searchRequest.SortingAndPagingParams.SortField;
            SortDirection = searchRequest.SortingAndPagingParams.SortDirection == Models.SortDirection.Asc ? "asc" : "desc";

            if (reader != null && reader.HasRows)
            {
                reader.Read();
                Total = Convert.ToInt32(reader["_totalCount"]);
                Tickets.Add(new DbSearchRecord(reader, searchRequest.DisplayFields, this));

                while (reader.Read())
                    Tickets.Add(new DbSearchRecord(reader, searchRequest.DisplayFields, this));
            }
        }

        private DbSearchResponse() { }

        public DbSearchResponse ReduceToOneTicket(DbSearchRecord ticket)
        {
            var newTicketSet = new DbSearchResponse()
            {
                PageNumber = 1,
                PageSize = PageSize,
                Total = 1,
                Request = ticket.Response.Request
            };

            newTicketSet.Tickets.Add(ticket);

            return newTicketSet;
        }


    }
}
