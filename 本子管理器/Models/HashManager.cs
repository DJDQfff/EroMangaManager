using System.IO;
using System.Threading.Tasks;

using EroMangaManager.Database.DatabaseOperation;
using EroMangaManager.Helpers;

namespace EroMangaManager.Models
{
    public static class HashManager
    {
        /// <summary>
        /// 查询数据库，是否含有对应长度的ZipArchiveEntry。含有则返回true
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static bool WhetherDatabaseMatchLength (long length)
        {
            int count = HashOperation.LengthConditionCount(length);
            return count != 0;
        }

        /// <summary>
        /// 查询数据库，是否含有对应Stream的Hash的ZipArchiveEntry。含有则返回true
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool StreamHashFilter (Stream stream)
        {
            string hash = stream.ComputeHash();
            int count = HashOperation.HashConditionCount(hash);
            return count != 0;
        }

        public static async Task Add (string hash)
        {
            await HashOperation.Add(hash);
        }

        public static async Task Remove (string[] hashes)
        {
            await HashOperation.Remove(hashes);
        }
    }
}