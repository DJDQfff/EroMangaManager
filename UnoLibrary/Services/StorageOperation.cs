using CommunityToolkit.Mvvm.Input;

namespace WinApp.Services;

public partial class StorageOperation(
    ObservableCollectionVM collectionVM,
    Window window,
    Exporter exporter
)
{
    [RelayCommand]
    public async Task ExportAsPDFAsync(Manga mangaBook)
    {
        FileSavePicker fileSavePicker = new();
        fileSavePicker.FileTypeChoices.Add("PDF", [".pdf"]);
        fileSavePicker.SuggestedFileName = mangaBook.FileDisplayName;

        var handle = WindowNative.GetWindowHandle(window);
        InitializeWithWindow.Initialize(fileSavePicker, handle);

        var storageFile = await fileSavePicker.PickSaveFileAsync();
        if (storageFile is null)
        {
            return;
        }
        try
        {
            await Task.Run(() => exporter.Export_PDFSharp(mangaBook, storageFile.Path));
            var done = StringsExtension.ResourceLoader.GetString("ExportDone");
            if (done != null)
            {
                collectionVM.WorkDone(done);
            }
        }
        catch
        {
            var failed = StringsExtension.ResourceLoader.GetString("ExportFailed");
            if (failed != null)
            {
                collectionVM.WorkFailed(failed);
            }
        }
    }

    public async Task Delete(
        Manga manga,
        StorageDeleteOption deletemode = StorageDeleteOption.Default
    )
    {
        switch (manga.Type)
        {
            case "":
                {
#if WINDOWS
                    var folder = await StorageFolder.GetFolderFromPathAsync(manga.FilePath);

                    await folder.DeleteAsync(deletemode);
#else
                    System.IO.Directory.Delete(manga.FilePath, true);
#endif
                }
                break;

            default:
                {
#if WINDOWS
                    var file = await StorageFile.GetFileFromPathAsync(manga.FilePath);

                    await file.DeleteAsync(deletemode);
#else
                    System.IO.File.Delete(manga.FilePath);
#endif
                }
                break;
        }
    }

    public async Task<bool> Delete(Manga manga)
    {
        try
        {
            switch (manga.Type)
            {
                case "":
                    {
#if WINDOWS
                        var folder = await StorageFolder.GetFolderFromPathAsync(manga.FilePath);

                        await folder.DeleteAsync(StorageDeleteOption.Default);
#else
                        System.IO.Directory.Delete(manga.FilePath, true);
#endif
                    }
                    break;

                default:
                    {
#if WINDOWS
                        var file = await StorageFile.GetFileFromPathAsync(manga.FilePath);

                        await file.DeleteAsync(StorageDeleteOption.Default);
#else
                        System.IO.File.Delete(manga.FilePath);
#endif
                    }
                    break;
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            return false;
        }
    }
}
