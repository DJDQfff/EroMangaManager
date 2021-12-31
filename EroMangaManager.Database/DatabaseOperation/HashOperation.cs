using System;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;

namespace EroMangaManager.Database.DatabaseOperation
{
    public static class HashOperation
    {
        public static int LengthConditionCount (long length)
        {
            Tables.Databases table = new Tables.Databases();

            var query = table.ImageFilters.Where(n => n.ZipEntryLength == length).Count();
            return query;
        }

        public static int HashConditionCount (string hash)
        {
            Tables.Databases table = new Tables.Databases();
            var query = 6;
            query = table.ImageFilters.Where(n => n.Hash == hash).Count();
            return query;
        }

        public static async Task Add (string hash)
        {
            Tables.Databases table = new Tables.Databases();
            ImageFilter imageHash = new ImageFilter()
            {
                Hash = hash,
            };
            table.Add(imageHash);
            await table.SaveChangesAsync();
        }

        public static async Task Remove (string[] hashes)
        {
            Tables.Databases table = new Tables.Databases();
            var h = table.ImageFilters.Where(n => hashes.Contains(n.Hash)).ToArray();
            table.RemoveRange(h);
            await table.SaveChangesAsync();
        }
    }
}