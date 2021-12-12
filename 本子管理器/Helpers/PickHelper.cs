using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
