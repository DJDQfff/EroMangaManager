using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using EroMangaDB.Entities;

using EroMangaManager.Core.Models;

using SharpCompress.Archives;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.UWP.SettingEnums.FolderEnum;
using static MyLibrary.UWP.StorageFolderHelper;

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
        public static async Task ChangeCoverFromTempFolder(this MangaBook mangaBook)
        {
            StorageFolder coverfolder = await GetChildTemporaryFolder(nameof(Covers));

            var file = await App.Current.storageItemManager.GetStorageFile(mangaBook.FilePath);
            var cover = await coverfolder.TryGetItemAsync(file.DisplayName + ".jpg");

            if (cover != null)
            {      
                // 以前是在MangaBook类里面放一个封面图像缓存，现在改为使用封面图像文件路径，这个方法也废弃

                //Windows.Storage.StorageFile StorageFile = cover as Windows.Storage.StorageFile;
                //BitmapImage BitmapImage = await CoverHelper.GetCoverThumbnail_SystemAsync(StorageFile);

                //mangaBook.Cover = BitmapImage;
            }
        }

        private static SvgImageSource _imageSource;

        /// <summary>
        /// 初始化默认漫画封面
        /// </summary>
        public static void InitialDefaultCover()
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
        /// <param name="filteredImages">要比较的数据</param>
        /// <returns></returns>
        public static async Task<string> CreatCoverFile_Origin_SharpCompress(this StorageFile storageFile, FilteredImage[] filteredImages)
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
                        bool canuse = entry.EntryFilter(filteredImages);
                        if (canuse)
                        {
                            path = Path.Combine(coverfolder.Path, storageFile.DisplayName + ".jpg");
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
        public static async Task<BitmapImage> GetCoverThumbnail_SystemAsync(this StorageFile cover)
        {
            BitmapImage bitmapImage = new BitmapImage();
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
        /// 清除所有封面文件
        /// </summary>
        /// <returns></returns>
        public static async Task ClearCovers()
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
        public static async Task<string> TryCreatCoverFileAsync(StorageFile storageFile, FilteredImage[] filteredImages)
        {
            string path;
            StorageFolder folder = await GetChildTemporaryFolder(nameof(Covers));
            IStorageItem storageItem = await folder.TryGetItemAsync(storageFile.DisplayName + ".jpg");
            if (storageItem is null)
            {
                path = await CoverHelper.CreatCoverFile_Origin_SharpCompress(storageFile, filteredImages);
            }
            else
            {
                path = storageItem.Path;
            }

            return path;
        }

    }
}