using CMS.DAL.Interfaces;
using CMS.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Services
{
    internal class DbTicketLinkCache : DbCacheService
    {
        public List<TicketLink> GetTicketLinksForFields(IEnumerable<Field> fields)
        {
            foreach (var field in fields)
            {
                var key = $"TicketLinkForField_{field.Id}";

                //GetObjectFromCache(key, 600, () => new DbDictionaryService().GetDictionaryRecords(dict));
            }

            return null;
        }
    }
}
