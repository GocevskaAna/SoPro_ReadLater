using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReadLater.Entities;

namespace ReadLater.Data
{
    public class ReadLaterDataContext : DbContext, IDbContext
    {
        static ReadLaterDataContext()
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<ReadLaterDataContext>());
        }

        public ReadLaterDataContext()
            : base("Name=ReadLaterDataContext")
        {
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public override int SaveChanges()
        {
            this.ApplyStateChanges();
            return base.SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            EntityTypeConfiguration<Category> categoryMap = modelBuilder.Entity<Category>();
            EntityTypeConfiguration<Bookmark> bookmarkMap = modelBuilder.Entity<Bookmark>();
            EntityTypeConfiguration<CategoriesPerUser> categoriesPerUserMap = modelBuilder.Entity<CategoriesPerUser>();
        }

        public System.Data.Entity.DbSet<ReadLater.Entities.Category> Categories { get; set; }
        public System.Data.Entity.DbSet<ReadLater.Entities.CategoriesPerUser> CategoriesPerUser { get; set; }

        public System.Data.Entity.DbSet<ReadLater.Entities.Bookmark> Bookmarks { get; set; }
    }
}
