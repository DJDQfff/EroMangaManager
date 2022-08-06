using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaTagDatabase.Helper;
using EroMangaTagDatabase.Tables;

using Microsoft.EntityFrameworkCore;

namespace EroMangaTagDatabase
{
    public partial class BasicController : IDisposable
    {
        public static BasicController DatabaseController;
        private Database database;

        static BasicController ()
        {
            DatabaseController = new BasicController();
        }

        private BasicController ()
        {
            database = new Database();
        }

        public void Dispose ()
        {
            database.Dispose();
            GC.SuppressFinalize(database);
        }
    }
}