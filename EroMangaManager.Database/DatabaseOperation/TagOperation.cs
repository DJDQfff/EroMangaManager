using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;

namespace EroMangaManager.Database.DatabaseOperation
{
    /// <summary>
    /// 没有保证Tag数据唯一性，因为在CollectionObserver类实际使用中，是能够保证唯一性的
    /// </summary>
    public static class TagOperation
    {
        public static async Task SaveTags (IList<MangaTag> mangaTags)
        {
            Tables.Databases databases = new Tables.Databases();
            databases.AddRange(mangaTags);
            await databases.SaveChangesAsync();
        }

        public static async Task SaveTag (MangaTag mangaTag)
        {
            Tables.Databases databases = new Tables.Databases();
            databases.Add(mangaTag);
            await databases.SaveChangesAsync();
        }

        public static async Task RemoveTags (IEnumerable<string> absolutePathes)
        {
            Tables.Databases databases = new Tables.Databases();

            List<MangaTag> list = new List<MangaTag>();
            foreach (var path in absolutePathes)
            {
                MangaTag mangaTag = databases.MangaTags.Where(n => n.AbsolutePath == path).Single();

                list.Add(mangaTag);
            }
            databases.RemoveRange(list);
            await databases.SaveChangesAsync();
        }

        public static async Task RemoveTag (string path)
        {
            Tables.Databases databases = new Tables.Databases();
            MangaTag mangaTag = databases.MangaTags.Single(n => n.AbsolutePath == path);
            databases.Remove(mangaTag);
            await databases.SaveChangesAsync();
        }

        public static MangaTag QueryTag (string absolutePath)
        {
            Tables.Databases databases = new Tables.Databases();
            var tag = databases.MangaTags.Where(n => n.AbsolutePath == absolutePath).Single();
            return tag;
        }

        public static async Task UpdateTag (MangaTag mangaTag)
        {
            Tables.Databases databases = new Tables.Databases();
            var tag = databases.MangaTags.Where(n => n.AbsolutePath == mangaTag.AbsolutePath).Single();
            databases.Remove(tag);
            databases.Add(mangaTag);
            await databases.SaveChangesAsync();
        }
    }
}