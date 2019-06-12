using CMS.DAL.Models;
using Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Services
{
    internal class DbTicketLinkService : DbUpdateService
    {
        

        public List<TicketLink> GetTicketLinks(IEnumerable<Field> fields)
        {
            var fieldIds = fields.Select(f => f.Id).ToList();

            using (var db = new CMSContext())
            {
                return db.TicketLinks
                    .Where(tl => fieldIds.Contains(tl.FieldId)).ToList();
            }
        }
    }
}
