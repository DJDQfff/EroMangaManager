using System;
using System.Linq;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;

namespace EroMangaTagDatabase.DatabaseOperation
{
    public static class HashOperation
    {
        public static int LengthConditionCount (long length)
        {
            Tables.Database table = new Tables.Database();

            var query = table.ImageFilters.Where(n => n.ZipEntryLength == length).Count();

            table.Dispose();

            return query;
        }

        public static int HashConditionCount (string hash)
        {
            Tables.Database table = new Tables.Database();
            var query = table.ImageFilters.Where(n => n.Hash == hash).Count();
            table.Dispose();

            return query;
        }

        public static async Task Add (string hash)
        {
            Tables.Database table = new Tables.Database();
            ImageFilter imageHash = new ImageFilter()
            {
                Hash = hash,
            };
            table.Add(imageHash);
            await table.SaveChangesAsync();
            table.Dispose();
        }

        public static async Task Remove (string[] hashes)
        {
            Tables.Database table = new Tables.Database();
            var h = table.ImageFilters.Where(n => hashes.Contains(n.Hash)).ToArray();
            table.RemoveRange(h);
            await table.SaveChangesAsync();
            table.Dispose();
        }
    }
}