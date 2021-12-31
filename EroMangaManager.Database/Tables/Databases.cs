using EroMangaManager.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaManager.Database.Tables
{
    public class Databases : DbContext
    {
        public DbSet<ImageFilter> ImageFilters { set; get; }
        public DbSet<MangaTag> MangaTags { set; get; }
        public DbSet<UserTagFilter> TagFilters { set; get; }
        public DbSet<ReadingInfo> ReadingRecords { set; get; }

        private readonly string ConnectionString;

        public Databases () => ConnectionString = "Data Source=Database.db";

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(ConnectionString);
        }

        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}