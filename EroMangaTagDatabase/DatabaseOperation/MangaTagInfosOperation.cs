using System.Collections.Generic;
using System.Linq;
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
        public async Task MangaTag_AddMulti (IList<MangaTagInfo> mangaTags)
        {
            database.AddRange(mangaTags);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// MangaTag表添加单个
        /// </summary>
        /// <param name="mangaTag"></param>
        /// <returns></returns>
        public async Task MangaTag_AddSingle (MangaTagInfo mangaTag)
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
            List<MangaTagInfo> list = new List<MangaTagInfo>();
            foreach (var path in absolutePathes)
            {
                MangaTagInfo mangaTag = database.ManTagInfos.Single(n => n.AbsolutePath == path);

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
            MangaTagInfo mangaTag = database.ManTagInfos.SingleOrDefault(n => n.AbsolutePath == path);
            database?.Remove(mangaTag);
            await database.SaveChangesAsync();
            database.Dispose();
        }

        /// <summary>
        /// MangaTag表查询多个
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public MangaTagInfo[] MangaTag_QueryMulti (string folder)
        {
            Tables.DataBase_1 database = new Tables.DataBase_1();
            var tags = database.ManTagInfos.Where(n => n.AbsolutePath.Contains(folder)).ToArray();

            return tags;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <returns></returns>
        public MangaTagInfo MangaTag_QuerySingle (string absolutePath)
        {
            var tag = database.ManTagInfos.Single(n => n.AbsolutePath == absolutePath);

            return tag;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mangaTag"></param>
        /// <returns></returns>
        public async Task MangaTag_UpdateSingle (MangaTagInfo mangaTag)
        {
            var tag = database.ManTagInfos.Single(n => n.AbsolutePath == mangaTag.AbsolutePath);
            database.Remove(tag);
            database.Add(mangaTag);
            await database.SaveChangesAsync();
        }
    }
}