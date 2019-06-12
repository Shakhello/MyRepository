using CMS.DAL.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CMS.DAL.Services
{
    public class CMSContext : DbContext
    {
        public DbSet<Event> Events { get; set; }

        public DbSet<Action> Actions { get; set; }

        public DbSet<Parameter> Parameters { get; set; }

        public DbSet<Control> Controls { get; set; }

        public DbSet<ControlField> ControlFields { get; set; }

        public DbSet<Field> Fields { get; set; }

        public DbSet<View> Views { get; set; }

        public DbSet<Filter> Filters { get; set; }

        public DbSet<FilterField> FilterFields { get; set; }

        public DbSet<Template> Templates { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Dictionary> Dictionaries { get; set; }

        public DbSet<DictionaryLink> DictionaryLinks { get; set; }

        public DbSet<FileLink> FileLinks { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<TicketLink> TicketLinks { get; set; }

        public DbSet<Style> Styles { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public CMSContext() : base("CMSContext")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder
                .Entity<Template>()
                .HasMany(t => t.Fields)
                .WithRequired(f => f.Template)
                .WillCascadeOnDelete(true);

            modelBuilder
                .Entity<View>()
                .HasMany(v => v.Controls)
                .WithRequired(c => c.View)
                .WillCascadeOnDelete(true);

            modelBuilder
                .Entity<View>()
                .HasMany(v => v.Permissions)
                .WithOptional(p => p.View)
                .WillCascadeOnDelete(true);

            //modelBuilder
            //    .Entity<Field>()
            //    .HasMany(f => f.Controls)
            //    .WithOptional(c => c.Field)
            //    .WillCascadeOnDelete(true);

            modelBuilder
                .Entity<Event>()
                .HasMany(e => e.Actions)
                .WithRequired(a => a.Event)
                .WillCascadeOnDelete(true);

            modelBuilder
                .Entity<Action>()
                .HasMany(a => a.Parameters)
                .WithRequired(p => p.Action)
                .WillCascadeOnDelete(true);

            modelBuilder
                .Entity<DictionaryLink>()
                .HasIndex(dl => new { dl.FieldId });

            modelBuilder
                .Entity<DictionaryLink>()
                .HasIndex(dl => new { dl.FieldId, dl.DocId });

            modelBuilder
                .Entity<TicketLink>()
                .HasIndex(dl => new { dl.FieldId, dl.DocId1 });

        }
    }
}
