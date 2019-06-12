using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Incoming
{
    public class Document
    {
        public int TemplateId { get; set; }

        public Dictionary<string, object> Values { get; set; }
    }
}