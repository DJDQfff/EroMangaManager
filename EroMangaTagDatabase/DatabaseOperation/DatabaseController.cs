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
    public partial class BasicController : IDisposable
    {
        public static BasicController DatabaseController;
        private DataBase_1 database;

        static BasicController ()
        {
            DatabaseController = new BasicController();
        }

        private BasicController ()
        {
            database = new DataBase_1();
        }
        public void Migrate ()
        {
            database.Database.Migrate();
        }
        public void Dispose ()
        {
            database.Dispose();
            GC.SuppressFinalize(database);
        }
    }
}