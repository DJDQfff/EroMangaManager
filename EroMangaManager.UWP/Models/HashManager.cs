using System.IO;
using System.Threading.Tasks;

using static EroMangaDB.BasicController;
using static  MyLibrary.Standard20.HashComputer;

namespace EroMangaManager.UWP.Models
{
    /// <summary>
    /// Hash帮助类
    /// </summary>
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

        /// <summary>
        /// 添加一个要筛除的hashlength
        /// </summary>
        /// <param name="hash"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static async Task Add (string hash , long length)
        {
            await DatabaseController.ImageFilter_Add(hash , length);
        }

        /// <summary>
        /// 移除集合hash
        /// </summary>
        /// <param name="hashes"></param>
        /// <returns></returns>
        public static async Task Remove (string[] hashes)
        {
            await DatabaseController.ImageFilter_Remove(hashes);
        }
    }
}