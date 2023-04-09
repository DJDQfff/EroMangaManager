using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using static MyLibrary.Standard20.HashComputer;


using SharpCompress.Archives;

using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaDB.BasicController;

namespace EroMangaManager.UWP.Helpers
{
    /// <summary>
    /// 压缩文件帮助类
    /// </summary>
    public static class ZipEntryHelper
    {

        /// <summary>
        /// 获取bitmapimage
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> ShowEntryAsync(IArchiveEntry entry)
        {
            using (Stream stream1 = entry.OpenEntryStream())
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream1.CopyTo(memoryStream);

                    using (IRandomAccessStream randomAccessStream = memoryStream.AsRandomAccessStream())
                    {
                        randomAccessStream.Seek(0);//记得偏移量归零，

                        BitmapImage bitmapImage = new BitmapImage();

                        await bitmapImage.SetSourceAsync(randomAccessStream);

                        return bitmapImage;
                    }
                }
            }
        }

        /// <summary>
        /// 筛选zipentry
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="isfilterimageon">TODO 是否过滤图片</param>
        /// <returns></returns>
        public static bool EntryFilter(this SharpCompress.Archives.IArchiveEntry entry, bool isfilterimageon)
        {
            bool canuse = true;

            if (entry.IsDirectory)                      // 排除文件夹entry
                return false;

            string extension = Path.GetExtension(entry.Key).ToLower();

            if (extension != ".jpg" && extension != ".png")
            {
                return false;
            }

            if (isfilterimageon)            // 是否检查图片过滤
            {
                #region 第一个条件：比较数据库，解压后大小

                if (DatabaseController.ImageFilter_LengthConditionCount(entry.Size) == 0)
                    return true;

                #endregion 第一个条件：比较数据库，解压后大小

                using (Stream stream = entry.OpenEntryStream())               // 不能对stream设置position
                {
                    #region // 第二个条件： 计算流hash，判断唯一性

                    string hash = stream.ComputeHash();
                    int count = DatabaseController.ImageFilter_HashConditionCount(hash);

                    if (count!=0)
                    {
                        canuse = false;
                    }

                    #endregion // 第二个条件： 计算流hash，判断唯一性
                }
            }

            return canuse;                                                  // 最后一定符合调教
        }

        /// <summary>
        /// 排序zipentry
        /// </summary>
        /// <param name="zipArchive"></param>
        /// <param name="sortFunc"></param>
        /// <returns></returns>
        public static IEnumerable<string> SortEntriesByName(this IArchive zipArchive, Action<IEnumerable<string>> sortFunc = null)
        {
            List<string> vs = new List<string>();
            foreach (var zipEntry in zipArchive.Entries)
            {
                string entryName = zipEntry.Key;
                vs.Add(entryName);
            }

            if (sortFunc != null)
            {
                sortFunc(vs);
            }
            else
            {
                vs.Sort();
            }

            return vs;
        }



    }
}