using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Tables;

using Microsoft.EntityFrameworkCore.Design;

namespace EroMangaManager.Database.DatabaseOperation
{
    public static class ReadingInfoTableOperation
    {
        public static async Task AddMulti (IEnumerable<ReadingInfo> ts)
        {
            Databases databases = new Databases();

            await databases.AddRangeAsync(ts);
            await databases.SaveChangesAsync();
        }

        public static async Task RemoveSingle (string path)
        {
            Databases databases = new Databases();
            var info = databases.ReadingRecords.SingleOrDefault(n => n.AbsolutePath == path);
            if (info != null)
            {
                databases.Remove(info);
                await databases.SaveChangesAsync();
            }
        }

        public static async Task UpdateSingle (ReadingInfo t)
        {
            Databases databases = new Databases();
            var info = databases.ReadingRecords.SingleOrDefault(n => n.AbsolutePath == t.AbsolutePath);
            if (info != null)
            {
                databases.Remove(info);
                databases.Add(info);
                await databases.SaveChangesAsync();
            }
        }

        public static async Task MultiTranslateMangaName (IEnumerable<(string path, string translatedname)> tuples)
        {
            Databases databases = new Databases();
            var info = databases.ReadingRecords.ToList();
            foreach (var tuple in tuples)
            {
                var a = info.SingleOrDefault(n => n.AbsolutePath == tuple.path);
                a.TranslatedMangaName = tuple.translatedname;
            }
            databases.UpdateRange(info);
            await databases.SaveChangesAsync();
        }

        public static async Task<ReadingInfo[]> QueryAll ()
        {
            Databases databases = new Databases();
            var infos = databases.ReadingRecords.ToArray();
            return infos;
        }
    }
}