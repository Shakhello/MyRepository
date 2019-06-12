using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Models
{
    public class TicketLink : Entity
    {
        public int FieldId { get; set; }

        public int DocId1 { get; set; }

        public int DocId2 { get; set; }
    }
}
