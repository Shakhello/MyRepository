using CMS.DAL.Models;
using CMS.DAL.Services;
using System.Data.Entity;
using System.Linq;

namespace CMS.DAL.Behaviours
{
    internal class FieldFileBehaviour : ICRUDBehaviour<Field>
    {
        public void OnCreate(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }

        public void OnDelete(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            var fileLinks = db.FileLinks.Where(dl => dl.FieldId == entity.Id);

            db.FileLinks.RemoveRange(fileLinks);

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
