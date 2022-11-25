using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Entities;

namespace EroMangaDB
{
    /// <summary>
    /// 没有保证Tag数据唯一性，因为在CollectionObserver类实际使用中，是能够保证唯一性的
    /// </summary>
    public partial class BasicController
    {
        /// <summary>
        /// MangaTag表添加多行
        /// </summary>
        /// <param name="mangaTags"></param>
        /// <returns></returns>
        public async Task MangaTag_AddMulti (IList<DefaultMangaTag> mangaTags)
        {
            database.AddRange(mangaTags);
            await database.SaveChangesAsync();
        }
        /// <summary>
        /// MangaTag表添加单个
        /// </summary>
        /// <param name="mangaTag"></param>
        /// <returns></returns>
        public async Task MangaTag_AddSingle (DefaultMangaTag mangaTag)
        {
            database.Add(mangaTag);
            await database.SaveChangesAsync();
        }
        /// <summary>
        /// MangaTag表移除多行
        /// </summary>
        /// <param name="absolutePathes"></param>
        /// <returns></returns>
        public async Task MangaTag_RemoveMulti (IEnumerable<string> absolutePathes)
        {
            List<DefaultMangaTag> list = new List<DefaultMangaTag>();
            foreach (var path in absolutePathes)
            {
                DefaultMangaTag mangaTag = database.SpecificMangaTagDatas.Single(n => n.AbsolutePath == path);

                list.Add(mangaTag);
            }
            database.RemoveRange(list);
            await database.SaveChangesAsync();
        }

        /// <summary>
        ///  移除指定路径的MangaTag。如果存在，则执行；如果不存在，则无操作
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task MangaTag_RemoveSingle (string path)
        {
            DefaultMangaTag mangaTag = database.SpecificMangaTagDatas.SingleOrDefault(n => n.AbsolutePath == path);
            database?.Remove(mangaTag);
            await database.SaveChangesAsync();
            database.Dispose();
        }
        /// <summary>
        /// MangaTag表查询多个
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public DefaultMangaTag[] MangaTag_QueryMulti (string folder)
        {
            Tables.DataBase_1 database = new Tables.DataBase_1();
            var tags = database.SpecificMangaTagDatas.Where(n => n.AbsolutePath.Contains(folder)).ToArray();

            return tags;
        }

        public DefaultMangaTag MangaTag_QuerySingle (string absolutePath)
        {
            var tag = database.SpecificMangaTagDatas.Single(n => n.AbsolutePath == absolutePath);

            return tag;
        }

        public async Task MangaTag_UpdateSingle (DefaultMangaTag mangaTag)
        {
            var tag = database.SpecificMangaTagDatas.Single(n => n.AbsolutePath == mangaTag.AbsolutePath);
            database.Remove(tag);
            database.Add(mangaTag);
            await database.SaveChangesAsync();
        }
    }
}