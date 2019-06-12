using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Unity.DAL
{
    public class UnityContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        public UnityContext() : base("UnityContext")
        {

        }
    }
}