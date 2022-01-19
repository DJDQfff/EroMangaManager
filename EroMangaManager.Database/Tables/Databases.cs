using EroMangaManager.Database.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaManager.Database.Tables
{
    public class Databases : DbContext
    {
        /// <summary>
        /// 存储用户添加的不显示的图片的数据库表
        /// </summary>
        public DbSet<ImageFilter> ImageFilters { set; get; }

        /// <summary>
        /// 存贮特定的本子Tag的表
        /// </summary>
        public DbSet<MangaTag> SpecificMangaTagDatas { set; get; }

        public DbSet<TagKeywords> TagKeywords { set; get; }

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