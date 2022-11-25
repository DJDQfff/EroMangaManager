using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Entities;
using EroMangaDB.Tables;

using Microsoft.EntityFrameworkCore.Design;

namespace EroMangaDB
{
    public partial class BasicController
    {
        public async Task ReadingInfo_AddMulti (IEnumerable<ReadingInfo> ts)
        {
            await database.AddRangeAsync(ts);
            await database.SaveChangesAsync();
        }

        public async Task ReadingInfo_RemoveSingle (string path)
        {
            var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == path);
            if (info != null)
            {
                database.Remove(info);
                await database.SaveChangesAsync();
            }
        }

        public async Task ReadingInfo_UpdateSingle (ReadingInfo t)
        {
            var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == t.AbsolutePath);
            if (info != null)
            {
                database.Remove(info);
                database.Add(info);
                await database.SaveChangesAsync();
            }
        }

        public async Task ReadingInfo_UpdateMangaName (ReadingInfo readinginfo, string manganame)
        {
            var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == readinginfo.AbsolutePath);
            info.MangaName = manganame;
            database.Update(info);
            await database.SaveChangesAsync();
        }

        public async Task ReadingInfo_MultiTranslateMangaName (IEnumerable<(string path, string translatedname)> tuples)
        {
            var info = database.ReadingInfos.ToList();
            foreach (var tuple in tuples)
            {
                var a = info.SingleOrDefault(n => n.AbsolutePath == tuple.path);
                a.MangaName_Translated = tuple.translatedname;
            }
            database.UpdateRange(info);
            await database.SaveChangesAsync();
        }

        public ReadingInfo[] ReadingInfo_QueryAll ()
        {
            var infos = database.ReadingInfos.ToArray();

            return infos;
        }
    }
}