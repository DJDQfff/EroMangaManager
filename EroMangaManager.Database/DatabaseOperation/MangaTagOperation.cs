using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;

namespace EroMangaTagDatabase.DatabaseOperation
{
    /// <summary>
    /// 没有保证Tag数据唯一性，因为在CollectionObserver类实际使用中，是能够保证唯一性的
    /// </summary>
    public static class MangaTagOperation
    {
        public static async Task AddMultiTags (IList<MangaTag> mangaTags)
        {
            Tables.Database databases = new Tables.Database();
            databases.AddRange(mangaTags);
            await databases.SaveChangesAsync();
            databases.Dispose();

        }

        public static async Task AddSingleTag (MangaTag mangaTag)
        {
            Tables.Database databases = new Tables.Database();
            databases.Add(mangaTag);
            await databases.SaveChangesAsync();
            databases.Dispose();

        }

        public static async Task RemoveMultiTags (IEnumerable<string> absolutePathes)
        {
            Tables.Database databases = new Tables.Database();

            List<MangaTag> list = new List<MangaTag>();
            foreach (var path in absolutePathes)
            {
                MangaTag mangaTag = databases.SpecificMangaTagDatas.Single(n => n.AbsolutePath == path);

                list.Add(mangaTag);
            }
            databases.RemoveRange(list);
            await databases.SaveChangesAsync();
            databases.Dispose();

        }

        /// <summary>
        /// 数据库操作： 移除指定路径的MangaTag。如果存在，则执行；如果不存在，则无操作
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task RemoveSingleTag (string path)
        {
            Tables.Database databases = new Tables.Database();
            MangaTag mangaTag = databases.SpecificMangaTagDatas.SingleOrDefault(n => n.AbsolutePath == path);
            databases?.Remove(mangaTag);
            await databases.SaveChangesAsync();
            databases.Dispose();

        }

        public static MangaTag[] QueryMultiTags (string folder)
        {
            Tables.Database databases = new Tables.Database();
            var tags = databases.SpecificMangaTagDatas.Where(n => n.AbsolutePath.Contains(folder)).ToArray();
            databases.Dispose();

            return tags;
        }

        public static MangaTag QuerySingleTag (string absolutePath)
        {
            Tables.Database databases = new Tables.Database();
            var tag = databases.SpecificMangaTagDatas.Single(n => n.AbsolutePath == absolutePath);
            databases.Dispose();

            return tag;
        }

        public static async Task UpdateSingleTag (MangaTag mangaTag)
        {
            Tables.Database databases = new Tables.Database();
            var tag = databases.SpecificMangaTagDatas.Single(n => n.AbsolutePath == mangaTag.AbsolutePath);
            databases.Remove(tag);
            databases.Add(mangaTag);
            await databases.SaveChangesAsync();
            databases.Dispose();

        }
    }
}