using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

using EroMangaManager.Models;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace EroMangaManager.Helpers
{
    public static class CoverHelper
    {
        /// <summary> 创建封面文件 </summary>
        /// <param name="storageFile"> </param>
        /// <returns> </returns>
        public static async Task CreatCoverFile (this StorageFile storageFile)
        {
            StorageFolder coverfolder = await FoldersHelper.GetCoversFolder();

            // TODO：一个可能的bug，两个文件的displayname相同，但后缀不同（应该不会，文件选择器，只会挑选zip文件）
            // TODO：解决了一个bug：原来是手动创建一个文件，然后写入流，再添加后缀名，来实现解压并创建文件功能
            // 后来改进：通过system.io自带api直接解压文件，虽然原bug的解决方法没有找到，但此bug已解决
            //StorageFile coverfile = await coverfolder.CreateFileAsync(storageFile.DisplayName, CreationCollisionOption.ReplaceExisting);
            //Stream coverstream = await coverfile.OpenStreamForWriteAsync();

            using (Stream stream = await storageFile.OpenStreamForReadAsync())
            {
                using (ZipArchive zipArchive = new ZipArchive(stream))
                {
                    if (zipArchive.Entries.Count != 0)                                  // 非空压缩包
                    {
                        foreach (ZipArchiveEntry entry in zipArchive.Entries)
                        {
                            bool canuse = entry.EntryFilter();
                            if (canuse)
                            {
                                string path = Path.Combine(coverfolder.Path, storageFile.DisplayName + ".jpg");
                                entry.ExtractToFile(path);
                                break;
                            }
                        }
                    }
                }
            }
            //await coverfile.RenameAsync(storageFile.DisplayName + ".jpg");
        }

        /// <summary> 调用系统API，返回缩率图 </summary>
        /// <param name="cover"> </param>
        /// <returns> </returns>
        public static async Task<BitmapImage> GetCoverThumbnail_SystemAsync (this StorageFile cover)
        {
            BitmapImage bitmapImage = new BitmapImage();
            // TODO：对于刚创建的图片文件，尚未生成系统缩率图，此时返回的是通用图片缩率图，而不是具体图片的缩率图
            //thumbnailMode.picturemode有坑,缩略图不完整
            using (StorageItemThumbnail thumbnail = await cover.GetThumbnailAsync(ThumbnailMode.SingleItem, 80))
            {
                await bitmapImage.SetSourceAsync(thumbnail);
            }
            return bitmapImage;
        }

        /// <summary>
        /// 自己解析的缩率图，性能、美观、分辨率、尺寸拉伸方面可能没有系统缩率图好，但是不会出现系统缩率图的问题（如上所述），
        /// 更新：原bug消除，此功能不再需要
        /// </summary>
        /// <param name="cover"> </param>
        /// <returns> </returns>
        public static async Task<BitmapImage> GetCoverThumbnail_CustomAsync (this StorageFile cover)
        {
            BitmapImage bitmapImage = new BitmapImage();

            Stream stream = await cover.OpenStreamForReadAsync();

            IRandomAccessStream randomAccessStream = stream.AsRandomAccessStream();
            bitmapImage.DecodePixelWidth = 136;              // 136目前最佳
            bitmapImage.DecodePixelHeight = 192;             // 192

            await bitmapImage.SetSourceAsync(randomAccessStream);

            return bitmapImage;
        }
    }
}