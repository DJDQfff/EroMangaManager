using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using EroMangaManager.Models;

using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace EroMangaManager.Helpers
{
    public static class ZipEntryHelper
    {
        /// <summary> 过滤掉不属于本子的内容，如：汉化组信息、付款码等 </summary>
        /// <param name="entry"> </param>
        /// <returns> </returns>
        public static bool EntryFilter (this ZipArchiveEntry entry)
        {
            bool canuse = true;

            if (entry.FullName.EndsWith('/'))                      // 排除文件夹entry
                return false;

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

        public static async Task<BitmapImage> ShowEntryAsync (ZipArchiveEntry zipArchiveEntry)
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
    }
}