using EroMangaManager.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaManager.Database.Tables
{
    public class Databases : DbContext
    {
        public DbSet<DefaultTagFilter> TranslatorTagsFromUser { set; get; }
        public DbSet<DefaultTagFilter> TranslatorTagsFromShared { set; get; }
        public DbSet<DefaultTagFilter> AuthorTagsFromUser { set; get; }
        public DbSet<DefaultTagFilter> AuthorTagsFromShared { set; get; }
        public DbSet<ImageFilter> ImageFilters { set; get; }
        public DbSet<MangaTag> MangaTagDatas { set; get; }
        public DbSet<UserDefinedTag> UserDefinedTags { set; get; }
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