using EroMangaDB.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaDB.Tables
{
    public class DataBase_1 : DbContext
    {
        /// <summary>
        /// 存储用户添加的不显示的图片的数据库表
        /// </summary>
        public DbSet<ImageFilter> ImageFilters { set; get; }

        /// <summary>
        /// 存贮特定的本子Tag的表
        /// </summary>
        public DbSet<DefaultMangaTag> SpecificMangaTagDatas { set; get; }
        /// <summary>
        /// UniqueTagInRelation数据表
        /// </summary>
        public DbSet<UniqueTagInRelation> TagKeywords { set; get; }
        /// <summary>
        /// ReadingInfo数据表
        /// </summary>
        public DbSet<ReadingInfo> ReadingInfos { set; get; }

        private readonly string ConnectionString;

        /// <summary>
        /// 构造函数
        /// </summary>
        public DataBase_1 ()
        {
            ConnectionString = "Data Source=DataBase_1.db";
        }

        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(ConnectionString);
        }

    }
}