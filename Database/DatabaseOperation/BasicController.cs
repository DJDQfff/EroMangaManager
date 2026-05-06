using EroMangaDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using System;

namespace EroMangaDatabase
{
    /// <summary>
    /// 将DBContext实例包装在这个单例类里面
    /// </summary>
    public partial class BasicController : IDisposable
    {
        /// <summary>
        /// 单一实例
        /// </summary>
        public static BasicController DatabaseController;

        /// <summary>
        ///
        /// </summary>
        public DataBase_Version3 database;

        static BasicController()
        {
            DatabaseController = new BasicController();
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private BasicController()
        {
            database = new DataBase_Version3(DatabaseConfig.ConnectingString);
        }

        /// <summary>
        /// 数据库版本迁移
        /// </summary>
        public void Migrate()
        {
            database.Database.Migrate();
        }

        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public void Dispose()
        {
            database.Dispose();
            GC.SuppressFinalize(database);
        }
    }
}