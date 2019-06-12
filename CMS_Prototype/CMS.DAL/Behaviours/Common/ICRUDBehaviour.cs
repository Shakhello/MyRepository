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
    internal interface ICRUDBehaviour<TEntity> where TEntity : Entity
    {
        void OnCreate(TEntity entity, CMSContext db, DbContextTransaction transaction);

        void OnRead(TEntity entity, CMSContext db, DbContextTransaction transaction);

        void OnUpdate(TEntity entity, CMSContext db, DbContextTransaction transaction);

        void OnDelete(TEntity entity, CMSContext db, DbContextTransaction transaction);
    }
}
