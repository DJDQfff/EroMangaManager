using System.IO;
using System.Threading.Tasks;

using EroMangaDB;
using static EroMangaDB.BasicController;
using static MyStandard20Library.HashComputer;
using EroMangaManager.Helpers;

namespace EroMangaManager.Models
{
    public static class HashManager
    {
        /// <summary>
        /// 查询数据库，是否含有对应长度的ZipArchiveEntry。含有则返回true
        /// </summary>
        /// <param name="length"> </param>
        /// <returns> </returns>
        public static bool WhetherDatabaseMatchLength (long length)
        {
            int count = DatabaseController.ImageFilter_LengthConditionCount(length);
            return count != 0;
        }

        /// <summary>
        /// 查询数据库，是否含有对应Stream的Hash的ZipArchiveEntry。含有则返回true
        /// </summary>
        /// <param name="stream"> </param>
        /// <returns> </returns>
        public static bool StreamHashFilter (Stream stream)
        {
            string hash = stream.ComputeHash();
            int count = DatabaseController.ImageFilter_HashConditionCount(hash);
            return count != 0;
        }

        public static async Task Add (string hash, long length)
        {
            await DatabaseController.ImageFilter_Add(hash, length);
        }

        public static async Task Remove (string[] hashes)
        {
            await DatabaseController.ImageFilter_Remove(hashes);
        }
    }
}