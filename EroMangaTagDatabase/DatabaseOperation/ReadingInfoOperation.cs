using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB.Entities;

namespace EroMangaDB
{
    public partial class BasicController
    {
        /// <summary>
        /// ReadingInof表添加多个
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public async Task ReadingInfo_AddMulti(IEnumerable<ReadingInfo> ts)
        {
            await database.AddRangeAsync(ts);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// ReadingInfo移除单个
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task ReadingInfo_RemoveSingle(string path)
        {
            var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == path);
            if (info != null)
            {
                database.Remove(info);
                await database.SaveChangesAsync();
            }
        }

        /// <summary>
        /// ReadingInfo更新单个
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public async Task ReadingInfo_UpdateSingle(ReadingInfo t)
        {
            var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == t.AbsolutePath);
            if (info != null)
            {
                database.Remove(info);
                database.Add(info);
                await database.SaveChangesAsync();
            }
        }

        /// <summary>
        /// ReadingInfo表更新MangaName属性
        /// </summary>
        /// <param name="readinginfo"></param>
        /// <param name="manganame"></param>
        /// <returns></returns>
        public async Task ReadingInfo_UpdateMangaName(ReadingInfo readinginfo, string manganame)
        {
            var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == readinginfo.AbsolutePath);
            info.MangaName = manganame;
            database.Update(info);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// ReadingInfo表修改多个翻译本子名属性
        /// </summary>
        /// <param name="tuples"></param>
        /// <returns></returns>
        public async Task ReadingInfo_MultiTranslateMangaName(IEnumerable<(string path, string translatedname)> tuples)
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

        /// <summary>
        /// ReadingInfo表查询所有
        /// </summary>
        /// <returns></returns>
        public ReadingInfo[] ReadingInfo_QueryAll()
        {
            var infos = database.ReadingInfos.ToArray();

            return infos;
        }
    }
}