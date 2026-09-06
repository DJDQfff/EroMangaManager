using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.Input;
using WinApp.Services;
using WinApp.Views.ContentDialogPages;

namespace UnoLibrary.Services;

public partial class ContentDialogCreater(
    Window window,
    StorageOperation storageOperation,
    SettingViewModel setting,
    CoverHelper coverHelper
)
{
    [RelayCommand]
    public async Task OverviewInformation(Manga manga)
    {
        OverviewInformation dialog = new(manga, coverHelper) { XamlRoot = window.Content.XamlRoot };
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
