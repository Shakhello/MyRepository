using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Unity.DAL
{
    public class Log
    {
        public int Id { get; set; }

        public Guid? GlobalRequestId { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public int Type { get; set; }
    }
}