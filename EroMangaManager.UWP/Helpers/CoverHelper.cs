using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using EroMangaManager.UWP.Models;

using SharpCompress.Archives;

using SkiaSharp;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.UWP.Models.FolderEnum;
using static MyUWPLibrary.StorageFolderHelper;

namespace EroMangaManager.UWP.Helpers
{
    /// <summary>
    /// 封面帮助类
    /// </summary>
    public static class CoverHelper
    {
        /// <summary>
        /// 读取TemporaryFolder中的本子封面文件，作为封面源
        /// </summary>
        /// <returns> </returns>
        [Obsolete]
        public static async Task ChangeCoverFromTempFolder (this MangaBook mangaBook)
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));

            var cover = await coverfolder.TryGetItemAsync(mangaBook.StorageFile.DisplayName + ".jpg");

            if (cover != null)
            {
                Windows.Storage.StorageFile storageFile = cover as Windows.Storage.StorageFile;
                BitmapImage bitmapImage = await CoverHelper.GetCoverThumbnail_SystemAsync(storageFile);

                // 以前是在MangaBook类里面放一个封面图像缓存，现在改为使用封面图像文件路径，这个方法也废弃
                //mangaBook.Cover = bitmapImage;
            }
        }

        private static SvgImageSource _imageSource;

        /// <summary>
        /// 初始化默认漫画封面
        /// </summary>
        public static void InitialDefaultCover ()
        {
            _imageSource = new SvgImageSource(new Uri(DefaultCoverPath));
        }

        /// <summary>
        /// 默认书籍封面路径
        /// </summary>
        public static string DefaultCoverPath => "ms-appx:///Assets/SVGs/书籍.svg";

        /// <summary>
        /// 获取默认封面
        /// </summary>
        public static SvgImageSource DefaultCover => _imageSource;

        /// <summary>
        /// 使用SharpCompress类库创建源图片设为封面
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        public static async Task<string> CreatCoverFile_Origin_SharpCompress (this StorageFile storageFile)
        {
            string path = null;
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));
            Stream stream = await storageFile.OpenStreamForReadAsync();
            try
            {
                using (var zipArchive = ArchiveFactory.Open(stream))
                {
                    foreach (var entry in zipArchive.Entries)
                    {
                        bool canuse = entry.EntryFilter(true);
                        if (canuse)
                        {
                            path = Path.Combine(coverfolder.Path , storageFile.DisplayName + ".jpg");
                            entry.WriteToFile(path);
                            break;
                        }
                    }
                }
                return path;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 调用系统API，返回缩率图 </summary>
        /// <param name="cover"> </param>
        /// <returns> </returns>
        public static async Task<BitmapImage> GetCoverThumbnail_SystemAsync (this StorageFile cover)
        {
            BitmapImage bitmapImage = new BitmapImage();
            //thumbnailMode.picturemode有坑,缩略图不完整
            IRandomAccessStream randomAccessStream;

            using (StorageItemThumbnail thumbnail = await cover.GetThumbnailAsync(ThumbnailMode.SingleItem , 80))
            {
                randomAccessStream = thumbnail.CloneStream();
            }
            await bitmapImage.SetSourceAsync(randomAccessStream);

            return bitmapImage;
        }

        /// <summary>
        /// 清除所有封面文件
        /// </summary>
        /// <returns></returns>
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

        /// <summary> 尝试创建封面文件。 </summary>
        /// <returns> </returns>
        public static async Task<string> TryCreatCoverFileAsync (StorageFile storageFile)
        {
            string path;
            StorageFolder folder = await GetChildTemporaryFolder(nameof(Covers));
            IStorageItem storageItem = await folder.TryGetItemAsync(storageFile.DisplayName + ".jpg");
            if (storageItem is null)
            {
                try
                {
                    path = await CoverHelper.CreatCoverFile_Origin_SharpCompress(storageFile);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                // 这段代码有坑 在debug模式下，这个try -
                // catch块可以正常运行，在release模式下无法运行
                // SkiaSharp库存在Bug，某些正常图片无法解码

                ////try
                ////{
                ////    await CoverHelper.CreatThumbnailCoverFile_UsingSkiaSharp(storageFile);
                ////}
                ////catch (Exception ex)
                ////{
                ////    IStorageItem storageItem1 = await folder.TryGetItemAsync(storageFile.DisplayName + ".jpg");

                ////    await storageItem1?.DeleteAsync(StorageDeleteOption.PermanentDelete);

                ////    await CoverHelper.CreatOriginCoverFile_UsingZipArchiveEntry(storageFile);
                ////}
            }
            else
            {
                path = storageItem.Path;
            }

            return path;
        }

        #region 不用

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

        /// <summary> 创建 缩略图 作为封面文件 </summary>
        /// <param name="storageFile"> </param>
        /// <returns> </returns>
        public static async Task CreatCoverFile_Thumbnail_SkiaSharp (this StorageFile storageFile)
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
                                    SKBitmap sKBitmap1 = sKBitmap.Resize(new SKImageInfo(sKBitmap.Width , sKBitmap.Height) , SKFilterQuality.Low);

                                    // SkiaSharp.SKImage
                                    // sKImage =
                                    // SkiaSharp.SKImage.FromEncodedData(memoryStream);
                                    // SKBitmap sKBitmap1 =
                                    // SkiaSharp.SKBitmap.FromImage(sKImage);

                                    using (Stream writestream = await coverFIle.OpenStreamForWriteAsync())
                                    {
                                        sKBitmap1.Encode(writestream , SKEncodedImageFormat.Jpeg , 30);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 使用SharpZIpLib类库创建源封面文件
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        public static async Task CreatCoverFile_Origin_ISharpCodeSharpZipLib (this StorageFile storageFile)
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));
            Stream stream = await storageFile.OpenStreamForReadAsync();

            try
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(stream))
                {
                    if (zipFile.Count != 0)                                  // 非空压缩包
                    {
                        foreach (ICSharpCode.SharpZipLib.Zip.ZipEntry entry in zipFile)
                        {
                            bool canuse = entry.EntryFilter();
                            if (canuse)
                            {
                                string path = Path.Combine(coverfolder.Path , storageFile.DisplayName + ".jpg");
                                File.Create(path);
                                FileInfo fileInfo = new FileInfo(path);
                                FileStream writestream = fileInfo.OpenWrite();

                                using (MemoryStream ms = new MemoryStream(entry.ExtraData))
                                {
                                    ms.CopyTo(writestream);
                                }
                                writestream.Dispose();
                                //entry.ExtractToFile(path);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 使用.net类库创建 源图 作为封面文件 </summary>
        /// <param name="storageFile"> </param>
        /// <returns> </returns>
        public static async Task CreatCoverFile_Origin_SystemIOCompressionZipArchive (this StorageFile storageFile)
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));

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
            //ICSharpCode.SharpZipLib.Zip.ZipFile zipFile = null;
            //try
            //{
            //    zipFile = new ICSharpCode.SharpZipLib.Zip.ZipFile(stream);
            //}
            //catch (Exception ex)
            //{
            //    return;         // 遇到问题直接退出
            //}
            //finally
            //{
            //    zipFile?.Close();
            //}
            try
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
                                string path = Path.Combine(coverfolder.Path , storageFile.DisplayName + ".jpg");
                                entry.ExtractToFile(path);
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //await coverfile.RenameAsync(storageFile.DisplayName + ".jpg");
        }

        #endregion 不用
    }
}