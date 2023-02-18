using EroMangaDB.Entities;

using Microsoft.EntityFrameworkCore;

namespace EroMangaDB.Tables
{
    /// <summary>
    /// 数据库类
    /// </summary>
    public class DataBase_1 : DbContext
    {
        /// <summary>
        /// 存储用户添加的不显示的图片的数据库表
        /// </summary>
        public DbSet<FilteredImage> FilteredImages { set; get; }

        /// <summary>
        /// UniqueTagInRelation数据表
        /// </summary>
        public DbSet<TagCategory> TagCategorys { set; get; }

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