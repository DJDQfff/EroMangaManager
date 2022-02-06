using System;
using System.Linq;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;

namespace EroMangaTagDatabase.DatabaseOperation
{
    public partial class DatabaseController
    {
        public int ImageFilter_LengthConditionCount (long length)
        {
            var query = database.ImageFilters.Where(n => n.ZipEntryLength == length).Count();

            return query;
        }

        public int ImageFilter_HashConditionCount (string hash)
        {
            var query = database.ImageFilters.Where(n => n.Hash == hash).Count();

            return query;
        }

        public async Task ImageFilter_Add (string hash)
        {
            ImageFilter imageHash = new ImageFilter()
            {
                Hash = hash,
            };
            database.Add(imageHash);
            await database.SaveChangesAsync();
        }

        public async Task ImageFilter_Remove (string[] hashes)
        {
            var h = database.ImageFilters.Where(n => hashes.Contains(n.Hash)).ToArray();
            database.RemoveRange(h);
            await database.SaveChangesAsync();
        }
    }
}