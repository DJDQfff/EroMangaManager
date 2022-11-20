using EroMangaTagDatabase.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaTagDatabase.Tables
{
    public class Database : DbContext
    {
        /// <summary>
        /// 存储用户添加的不显示的图片的数据库表
        /// </summary>
        public DbSet<ImageFilter> ImageFilters { set; get; }

        /// <summary>
        /// 存贮特定的本子Tag的表
        /// </summary>
        public DbSet<DefaultMangaTag> SpecificMangaTagDatas { set; get; }

        public DbSet<RealUniqueTag> TagKeywords { set; get; }

        public DbSet<ReadingInfo> ReadingInfos { set; get; }

        private readonly string ConnectionString;

        public Database () => ConnectionString = "Data Source=Database.db";

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