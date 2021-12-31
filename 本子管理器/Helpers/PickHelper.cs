using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace EroMangaManager.Helpers
{
    public class PickHelper
    {
        public static async Task<StorageFolder> PickFolder ()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".");
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;

            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            return folder;
        }

        public static async Task<StorageFile> SavePicture ()
        {
            FileSavePicker file = new FileSavePicker();
            file.FileTypeChoices.Add("图片", new List<string>() { ".jpg", ".png" });
            file.SuggestedStartLocation = PickerLocationId.Desktop;
            StorageFile storageFile = await file.PickSaveFileAsync();
            return storageFile;
        }
    }
}