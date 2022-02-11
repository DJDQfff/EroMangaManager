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
    public partial class Controller : IDisposable
    {
        public static Controller DatabaseController;
        private Database database;

        static Controller ()
        {
            DatabaseController = new Controller();
        }

        private Controller ()
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