using EroMangaDatabase.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EroMangaDatabase.Tables
{
    /// <summary>
    /// 数据库类
    /// </summary>
    public class DataBase_Version3 : DbContext
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private readonly string ConnectionString;

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

        /// <summary>
        ///访问文件夹存储
        /// </summary>
        public DbSet<MangaFolder> MangaFolders { set; get; }

        /// <summary>
        ///访问文件夹存储
        /// </summary>
        public DbSet<UWPAccessIStorage> UWPAccessIStorages { set; get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="connectionString"></param>
        public DataBase_Version3(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// 配置数据库
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            Debug.WriteLine(ConnectionString);
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}