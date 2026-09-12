using UnoLibrary.ContentDialogPages;
using WinApp.Views.ContentDialogPages;

namespace UnoLibrary.Services;

public partial class ContentDialogCreater(
    Window window,
    StorageOperation storageOperation,
    SettingViewModel setting,
    CoverHelper coverHelper
)
{
    /// <summary>
    /// 删除源文件时，会触发删除确认弹框，删除模式，这两个参数都是从程序设置中读取的，因此封装到助手类里面
    /// </summary>
    /// <param name="eroManga"></param>
    /// <returns></returns>
    public async Task<bool> ConfirmDeleteSourceFileDialog(Manga eroManga)
    {
        var temp1 = setting.AppConfig.General.WhetherShowDialogBeforeDelete;

        var temp2 = setting.AppConfig.General.StorageFileDeleteOption;

        var deletemode = temp2 ? StorageDeleteOption.PermanentDelete : StorageDeleteOption.Default;

        if (!temp1)
        {
            ConfirmDeleteMangaFile confirm = new(eroManga) { XamlRoot = window.Content!.XamlRoot };
            var result = await confirm.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    await storageOperation.Delete(eroManga, deletemode);
                    return true;

                default:
                    return false;
            }
        }
        else
        {
            await storageOperation.Delete(eroManga, deletemode);

            return true;
        }
    }

    /// <summary>
    /// 修改文件名
    /// </summary>
    /// <param name="eroManga"></param>
    /// <returns></returns>
    public async Task RenameSourceFileInDialog(
        Manga eroManga,
        MangaIO mangaFileIO,
        ObservableCollectionVM observableCollectionVM
    )
    {
        // TODO 暂时放弃，不会写页面UI，写出来也丑。等EditTag功能好了，在改回EditTag页面
        RenameDialog renameDialog = new(eroManga, mangaFileIO, observableCollectionVM)
        {
            XamlRoot = window!.Content!.XamlRoot,
        };

        _ = await renameDialog.ShowAsync();
    }

    [RelayCommand]
    public async Task OverviewInformation(Manga manga)
    {
        OverviewInformation dialog = new(manga, coverHelper)
        {
            XamlRoot = window!.Content!.XamlRoot,
        };
        _ = await dialog.ShowAsync();
    }

    public async Task<StorageFile> PickSingleFile(string title, string fileType)
    {
        FileOpenPicker picker = new()
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            FileTypeFilter = { fileType },
            SettingsIdentifier = "EroManga",
            CommitButtonText = title,
        };
        var handle = WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, handle);
        var file = await picker.PickSingleFileAsync();
        return file;
    }
}
