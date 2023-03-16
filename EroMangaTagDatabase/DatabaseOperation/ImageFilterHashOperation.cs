using System;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB.Entities;

namespace EroMangaDB
{
    public partial class BasicController
    {
        /// <summary>
        /// 统计符合Length条件的个数
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public int ImageFilter_LengthConditionCount(long length)
        {
            var query = database.FilteredImages.Count(n => n.ZipEntryLength == length);

            return query;
        }

        /// <summary>
        /// 统计符合Hash条件的个数
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        public int ImageFilter_HashConditionCount(string hash)
        {
            var query = database.FilteredImages.Count(n => n.Hash == hash);

            return query;
        }

        /// <summary>
        /// ImageFilter表添加新行
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public async Task ImageFilter_Add(string hash, long length)
        {
            FilteredImage imageHash = new FilteredImage()
            {
                Hash = hash,
                ZipEntryLength = length
            };
            database.Add(imageHash);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// ImageFIlter表移除行
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public async Task ImageFilter_Remove(string[] hashes)
        {
            var h = database.FilteredImages.Where(n => hashes.Contains(n.Hash)).ToArray();
            database.RemoveRange(h);
            await database.SaveChangesAsync();
        }
    }
}