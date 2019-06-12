using CMS.DAL.Models;
using CMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DAL.Behaviours
{
    internal class FieldDictionaryBehaviour : ICRUDBehaviour<Field>
    {
        public void OnCreate(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }

        public void OnDelete(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            var dictionaryLinks = db.DictionaryLinks.Where(dl => dl.FieldId == entity.Id);

            db.DictionaryLinks.RemoveRange(dictionaryLinks);

            DbDictionaryCache.ClearForFields(new int[] { entity.Id });

            db.SaveChanges();
        }

        public void OnRead(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }

        public void OnUpdate(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }
    }
}
