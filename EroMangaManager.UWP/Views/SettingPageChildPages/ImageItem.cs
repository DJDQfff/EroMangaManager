using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.UWP.SettingEnums.FolderEnum;
using static MyLibrary.UWP.StorageFolderHelper;

namespace EroMangaManager.UWP.Views.SettingPageChildPages
{
    /// <summary>
    /// 文件信息类
    /// </summary>
    public class ImageItem
    {
        /// <summary>
        /// 文件
        /// </summary>
        public StorageFile StorageFile { set; get; }

        /// <summary>
        /// 图像
        /// </summary>
        public BitmapImage BitmapImage { set; get; } = new BitmapImage();

        private ImageItem(StorageFile storage)
        {
            StorageFile = storage;
        }

        private async Task InitializeImage()
        {
            var thumbnail = await StorageFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 80);
            await BitmapImage.SetSourceAsync(thumbnail);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static async Task GetsAsync(ObservableCollection<ImageItem> items)
        {
            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            var files = await storageFolder.GetFilesAsync();

            foreach (var file in files)
            {
                ImageItem imageItem = new ImageItem(file);
                await imageItem.InitializeImage();
                items.Add(imageItem);
            }
        }
    }
}