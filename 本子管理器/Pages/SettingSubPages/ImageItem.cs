using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace EroMangaManager.Pages.SettingSubPages
{
    public class ImageItem
    {
        public StorageFile storageFile { set; get; }
        public BitmapImage bitmapImage { set; get; } = new BitmapImage();

        private ImageItem (StorageFile storage)
        {
            storageFile = storage;
        }

        private async Task InitializeImage ()
        {
            var thumbnail = await storageFile.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, 80);
            await bitmapImage.SetSourceAsync(thumbnail);
        }

        public static async Task GetsAsync (ObservableCollection<ImageItem> items)
        {
            StorageFolder storageFolder = await FoldersHelper.GetFilterFolder();
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