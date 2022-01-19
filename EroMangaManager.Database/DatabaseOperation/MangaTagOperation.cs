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
    public static class MangaTagOperation
    {
        public static async Task AddMultiTags (IList<MangaTag> mangaTags)
        {
            Tables.Databases databases = new Tables.Databases();
            databases.AddRange(mangaTags);
            await databases.SaveChangesAsync();
        }

        public static async Task AddSingleTag (MangaTag mangaTag)
        {
            Tables.Databases databases = new Tables.Databases();
            databases.Add(mangaTag);
            await databases.SaveChangesAsync();
        }

        public static async Task RemoveMultiTags (IEnumerable<string> absolutePathes)
        {
            Tables.Databases databases = new Tables.Databases();

            List<MangaTag> list = new List<MangaTag>();
            foreach (var path in absolutePathes)
            {
                MangaTag mangaTag = databases.SpecificMangaTagDatas.Single(n => n.AbsolutePath == path);

                list.Add(mangaTag);
            }
            databases.RemoveRange(list);
            await databases.SaveChangesAsync();
        }

        /// <summary>
        /// 数据库操作： 移除指定路径的MangaTag。如果存在，则执行；如果不存在，则无操作
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task RemoveSingleTag (string path)
        {
            Tables.Databases databases = new Tables.Databases();
            MangaTag mangaTag = databases.SpecificMangaTagDatas.SingleOrDefault(n => n.AbsolutePath == path);
            databases?.Remove(mangaTag);
            await databases.SaveChangesAsync();
        }

        public static MangaTag[] QueryMultiTags (string folder)
        {
            Tables.Databases databases = new Tables.Databases();
            var tags = databases.SpecificMangaTagDatas.Where(n => n.AbsolutePath.Contains(folder)).ToArray();
            return tags;
        }

        public static MangaTag QuerySingleTag (string absolutePath)
        {
            Tables.Databases databases = new Tables.Databases();
            var tag = databases.SpecificMangaTagDatas.Single(n => n.AbsolutePath == absolutePath);
            return tag;
        }

        public static async Task UpdateSingleTag (MangaTag mangaTag)
        {
            Tables.Databases databases = new Tables.Databases();
            var tag = databases.SpecificMangaTagDatas.Single(n => n.AbsolutePath == mangaTag.AbsolutePath);
            databases.Remove(tag);
            databases.Add(mangaTag);
            await databases.SaveChangesAsync();
        }
    }
}