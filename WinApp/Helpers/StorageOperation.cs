namespace EroMangaManager.WinUI3.Helpers;

internal class StorageOperation
{
    internal static async Task ExportAsPDFAsync(Manga mangaBook)
    {
        var fileSavePicker = new FileSavePicker();
        fileSavePicker.FileTypeChoices.Add("PDF", [".pdf"]);
        fileSavePicker.SuggestedFileName = mangaBook.FileDisplayName;

        var handle = WindowNative.GetWindowHandle(App.Current.MainWindow);
        InitializeWithWindow.Initialize(fileSavePicker, handle);

        var storageFile = await fileSavePicker.PickSaveFileAsync();
        if (storageFile is null)
        {
            return;
        }
        try
        {
            await Task.Run(() => Exporter.Export_PDFSharp(mangaBook, storageFile.Path));
            string done = ResourceLoader.GetForViewIndependentUse().GetString("ExportDone");
            App.Current.GlobalViewModel.WorkDone(done);
        }
        catch
        {
            string failed = ResourceLoader.GetForViewIndependentUse().GetString("ExportFailed");
            App.Current.GlobalViewModel.WorkFailed(failed);
        }
    }


    internal static async Task Delete(Manga manga, StorageDeleteOption deletemode=StorageDeleteOption.Default)
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
    internal static async void Delete(Manga manga)
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

        }
        catch (Exception ex) {  
            Debug.WriteLine(ex.ToString());
        }
    }


}