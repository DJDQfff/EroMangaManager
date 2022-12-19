using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Linq;
using EroMangaManager.Models;

using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

using ICSharpCode.SharpZipLib.Zip;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Archives.GZip;

namespace EroMangaManager.Helpers
{
    /// <summary>
    /// 压缩文件帮助类
    /// </summary>
    public static class ZipEntryHelper
    {
        #region SharpCompress
        /// <summary>
        /// 获取bitmapimage
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> ShowEntryAsync (IArchiveEntry entry)
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
        public static bool EntryFilter (this SharpCompress.Archives.IArchiveEntry entry ,bool isfilterimageon)
        {
            bool canuse = true;

            if (entry.IsDirectory)                      // 排除文件夹entry
                return false;

            string extension = Path.GetExtension(entry.Key).ToLower();

            if (extension != ".jpg" && extension != ".png")
            {
                return false;
            }

            //return true; // TODO 临时关闭筛选功能
            if (isfilterimageon)            // 是否检查图片过滤
            {
            if (HashManager.WhetherDatabaseMatchLength(entry.Size))              // 第一个条件：比较数据库，解压后大小
                return false;

            using (Stream stream = entry.OpenEntryStream())               // 不能对stream设置position
            {
                if (HashManager.StreamHashFilter(stream))      // 第二个条件：计算流hash，判断唯一性
                {
                    canuse = false;                              // 符合以上条件，这个entry不会被过滤掉
                }
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
        public static IEnumerable<string> SortEntriesByName (this IArchive zipArchive, Action<IEnumerable<string>> sortFunc = null)
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

        #endregion SharpCompress

        #region System.IO.Compression
        /// <summary>
        /// 解析zipentry获取bitmapimage
        /// </summary>
        /// <param name="zipArchiveEntry"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> ShowEntryAsync (System.IO.Compression.ZipArchiveEntry zipArchiveEntry)
        {
            using (Stream stream1 = zipArchiveEntry.Open())
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

        /// <summary> 过滤掉不属于本子的内容，如：汉化组信息、付款码等 </summary>
        /// <param name="entry"> </param>
        /// <returns> </returns>
        public static bool EntryFilter (this System.IO.Compression.ZipArchiveEntry entry)
        {
            bool canuse = true;

            if (entry.FullName.EndsWith('/'))                      // 排除文件夹entry
                return false;

            string extension = Path.GetExtension(entry.Name).ToLower();

            if (extension != ".jpg" && extension != ".png")
            {
                return false;
            }
            //return true; // TODO 临时关闭筛选功能

            if (HashManager.WhetherDatabaseMatchLength(entry.Length))              // 第一个条件：比较数据库，解压后大小
                return false;

            using (Stream stream = entry.Open())               // 不能对stream设置position
            {
                if (HashManager.StreamHashFilter(stream))      // 第二个条件：计算流hash，判断唯一性
                {
                    canuse = false;                              // 符合以上条件，这个entry不会被过滤掉
                }
            }

            return canuse;                                                  // 最后一定符合调教
        }

        /// <summary> 挑选所有entry的Name，并进行排序 </summary>
        /// <param name="zipArchive"> 要获取的压缩文件类 </param>
        /// <param name="sortFunc">
        /// 排序方法。传参，则按给定方法排序；不传，则按List.Sort()方法排序
        /// </param>
        /// <returns> </returns>
        public static IEnumerable<string> SortEntriesByName (this System.IO.Compression.ZipArchive zipArchive, Action<IEnumerable<string>> sortFunc = null)
        {
            List<string> vs = new List<string>();
            for (int i = 0; i < zipArchive.Entries.Count; i++)
            {
                string entryName = zipArchive.Entries[i].FullName;
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

        #endregion System.IO.Compression

        #region ISharpCode.SharpZipLib
        /// <summary>
        /// 筛选zipentry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static bool EntryFilter (this ZipEntry entry)
        {
            bool canuse = true;

            if (entry.IsDirectory)                      // 排除文件夹entry
                return false;

            string extension = Path.GetExtension(entry.Name).ToLower();

            if (extension != ".jpg" && extension != ".png")
            {
                return false;
            }
            //return true; // TODO 临时关闭筛选功能
            if (HashManager.WhetherDatabaseMatchLength(entry.Size))            // 第一个条件：比较数据库，解压后大小
                return false;
            using (MemoryStream stream = new MemoryStream(entry.ExtraData)) // 不能对stream设置position
            {
                if (HashManager.StreamHashFilter(stream))      // 第二个条件：计算流hash，判断唯一性
                {
                    canuse = false;                              // 符合以上条件，这个entry不会被过滤掉
                }
            }

            return canuse;                                                  // 最后一定符合调教
        }

        #endregion ISharpCode.SharpZipLib
    }
}