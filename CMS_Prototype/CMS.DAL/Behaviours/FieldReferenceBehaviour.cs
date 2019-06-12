using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.DAL.Models;
using CMS.DAL.Services;

namespace CMS.DAL.Behaviours
{
    internal class FieldReferenceBehaviour : ICRUDBehaviour<Field>
    {
        public void OnCreate(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            
        }

        public void OnDelete(Field entity, CMSContext db, DbContextTransaction transaction)
        {
            var evilTwin = entity.LinkedField;

            db.Fields.Remove(evilTwin);
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
