﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

using static MyUWPLibrary.StorageFolderHelper;
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
            StorageFolder storageFolder = await GetChildTemporaryFolder("Filter");
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