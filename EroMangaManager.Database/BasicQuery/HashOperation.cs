using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Tables;

namespace EroMangaManager.Database.BasicQuery
{
    public static class HashOperation
    {
        public static Tables.Databases table = new Tables.Databases();

        public static int LengthConditionCount (long length)
        {
            var query = table.ImageFilterTable.Where(n => n.ZipEntryLength == length).Count();
            return query;
        }

        public static int HashConditionCount (string hash)
        {
            var query = 6;
            query = table.ImageFilterTable.Where(n => n.Hash == hash).Count();
            return query;
        }

        public static async Task Add (string hash)
        {
            ImageFilter imageHash = new ImageFilter()
            {
                Hash = hash,
            };
            table.Add(imageHash);
            await table.SaveChangesAsync();
        }

        public static async Task Remove (string[] hashes)
        {
            var h = table.ImageFilterTable.Where(n => hashes.Contains(n.Hash)).ToArray();
            table.RemoveRange(h);
            await table.SaveChangesAsync();
        }
    }
}