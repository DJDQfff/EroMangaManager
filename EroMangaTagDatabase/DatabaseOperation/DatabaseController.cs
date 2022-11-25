using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Entities;
using EroMangaDB.EntityFactory;
using EroMangaDB.Helper;
using EroMangaDB.Tables;

using Microsoft.EntityFrameworkCore;

namespace EroMangaDB
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
        private DataBase_1 database;

        static BasicController ()
        {
            DatabaseController = new BasicController();
        }

        /// <summary>
        /// 私有构造函数
        /// </summary>
        private BasicController ()
        {
            database = new DataBase_1();
        }

        /// <summary>
        /// 数据库版本迁移
        /// </summary>
        public void Migrate ()
        {
            database.Database.Migrate();
        }

        /// <summary>
        /// 释放数据库资源
        /// </summary>
        public void Dispose ()
        {
            database.Dispose();
            GC.SuppressFinalize(database);
        }
    }
}