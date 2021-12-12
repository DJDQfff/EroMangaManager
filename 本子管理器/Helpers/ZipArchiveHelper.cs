using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using EroMangaManager.Models;

namespace EroMangaManager.Helpers
{
    public static class ZipArchiveHelper
    {
        /// <summary> 过滤掉不属于本子的内容，如：汉化组信息、付款码等 </summary>
        /// <param name="entry"> </param>
        /// <returns> </returns>
        public static bool EntryFilter (this ZipArchiveEntry entry)
        {
            bool canuse = false;
            if (HashManager.LengthFilter(entry.Length, 0))              // 第一个条件：解压后大小符合条件，在vary范围了符合即可
            {
                if (!entry.FullName.EndsWith('/'))                      // 排除文件夹entry
                {
                    using (Stream stream1 = entry.Open())               // 不能对stream设置position
                    {
                        if (HashManager.StreamHashFilter(stream1))      // 第二个条件：计算流hash，判断唯一性
                        {
                            canuse = true;                              // 符合以上条件，这个entry不会被过滤掉
                        }
                    }
                }
            }
            return canuse;
        }

        public static async Task<int> CountPage (this StorageFile storageFile)
        {
            int count;
            using (Stream stream = await storageFile.OpenStreamForReadAsync())
            {
                using (ZipArchive zipArchive = new ZipArchive(stream))
                {
                    count = zipArchive.Entries.Count(n => n.Length != 0);
                }
            }
            return count;
        }

        public static async Task<List<ZipArchiveEntry>> GetEntriesAsync (StorageFile storageFile)
        {
            Stream stream = await storageFile.OpenStreamForReadAsync();
            ZipArchive zipArchive = new ZipArchive(stream);
            List<ZipArchiveEntry> zipArchiveEntries = new List<ZipArchiveEntry>();
            foreach (var entry in zipArchive.Entries)
            {
                if (entry.Length != 0)
                {
                    zipArchiveEntries.Add(entry);
                }
            }

            return zipArchiveEntries;
        }

        public static async Task<BitmapImage> OpenEntryAsync (ZipArchiveEntry zipArchiveEntry)
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