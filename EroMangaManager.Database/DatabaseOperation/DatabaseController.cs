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

namespace EroMangaTagDatabase.DatabaseOperation
{
    public partial class DatabaseController : IDisposable
    {
        public static DatabaseController databaseController;
        private Database database;

        static DatabaseController ()
        {
            databaseController = new DatabaseController();
        }

        private DatabaseController ()
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