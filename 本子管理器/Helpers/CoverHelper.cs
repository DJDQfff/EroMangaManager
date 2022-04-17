using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using SkiaSharp;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.Models.FolderEnum;
using static MyUWPLibrary.StorageFolderHelper;

namespace EroMangaManager.Helpers
{
    public static class CoverHelper
    {
        /// <summary> 创建 缩略图 作为封面文件 </summary>
        /// <param name="storageFile"> </param>
        /// <returns> </returns>
        public static async Task CreatThumbnailCoverFile_UsingSkiaSharp (this StorageFile storageFile)
        {
            StorageFolder coverFolder = await GetChildTemporaryFolder(nameof(Covers));
            StorageFile coverFIle = await coverFolder.CreateFileAsync(storageFile.DisplayName + ".jpg");

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
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    using (Stream entrystream = entry.Open())
                                    {
                                        entrystream.CopyTo(memoryStream);
                                    }

                                    memoryStream.Position = 0;

                                    // 存在bug，一些图片解码会返回null

                                    SkiaSharp.SKBitmap sKBitmap = SkiaSharp.SKBitmap.Decode(memoryStream);
                                    SKBitmap sKBitmap1 = sKBitmap.Resize(new SKImageInfo(sKBitmap.Width, sKBitmap.Height), SKFilterQuality.Low);

                                    // SkiaSharp.SKImage
                                    // sKImage =
                                    // SkiaSharp.SKImage.FromEncodedData(memoryStream);
                                    // SKBitmap sKBitmap1 =
                                    // SkiaSharp.SKBitmap.FromImage(sKImage);

                                    using (Stream writestream = await coverFIle.OpenStreamForWriteAsync())
                                    {
                                        sKBitmap1.Encode(writestream, SKEncodedImageFormat.Jpeg, 30);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary> 创建 原图 作为封面文件 </summary>
        /// <param name="storageFile"> </param>
        /// <returns> </returns>
        public static async Task CreatOriginCoverFile_UsingZipArchiveEntry (this StorageFile storageFile)
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));

            // TODO：一个可能的bug，两个文件的displayname相同，但后缀不同（应该不会，文件选择器，只会挑选zip文件）
            // 解决了一个bug：原来是手动创建一个文件，然后写入流，再添加后缀名，来实现解压并创建文件功能
            // 后来改进：通过system.io自带api直接解压文件，虽然原bug的解决方法没有找到，但此bug已解决
            //StorageFile coverfile = await coverfolder.CreateFileAsync(storageFile.DisplayName, CreationCollisionOption.ReplaceExisting);
            //Stream coverstream = await coverfile.OpenStreamForWriteAsync();

            // 不用复制流的话，如果文件正常，则正常使用；但如果文件不对，在new
            // ZipArchive（）时，会一直卡顿很长时间

            Stream stream = await storageFile.OpenStreamForReadAsync();

            // System.IO.Compression.ZipArchive
            // 实例化时，如果有异常，会出现卡死（只有在UWP有这个问题），所以用了SharpZipLib来测试一下文件的正确性
            // 懒得把整个都换掉
            ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = null;
            try
            {
                zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(stream);
            }
            catch (Exception ex)
            {
                throw ex;//Zip文件格式不正确
            }
            finally
            {
                zipFile?.Close();
            }

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
            IRandomAccessStream randomAccessStream;

            using (StorageItemThumbnail thumbnail = await cover.GetThumbnailAsync(ThumbnailMode.SingleItem, 80))
            {
                randomAccessStream = thumbnail.CloneStream();
            }
            await bitmapImage.SetSourceAsync(randomAccessStream);

            return bitmapImage;
        }

        /// <summary>
        /// 系统缩率图存在更新迟缓的问题，还是得靠自己写缩率图 ，此功能不再需要
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

        public static async Task ClearCovers ()
        {
            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Covers));
            var files = await storageFolder.GetFilesAsync();
            List<Task> tasks = new List<Task>();

            foreach (var file in files)
            {
                tasks.Add(Task.Run(() => file.DeleteAsync(StorageDeleteOption.PermanentDelete)));
            }

            await Task.WhenAll(tasks);
        }
    }
}