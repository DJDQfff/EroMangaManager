using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Windows.AppNotifications.Builder;

namespace WinApp;

public partial class MangaOperationViewModel(
    CoverHelper coverHelper,
    ClipboardHelper clipboardHelper,
    StorageOperation storageOperation,
    DialogHelper dialogHelper,
    ObservableCollectionVM observableCollectionVM,
    MangaFactory mangaFactory,
    SettingViewModel settingViewModel,
    MangaFileIO mangaFileIO
) : ObservableObject
{
    public ObservableCollection<string> ExePaths => settingViewModel.ExePaths;
    public SvgImageSource ErrorImage => coverHelper.ErrorCoverImage;
    public ObservableCollection<MangasGroup> MangasGroups => observableCollectionVM.MangasGroups;

    [RelayCommand]
    private async Task OverviewInformation(Manga manga)
    {
        await dialogHelper.OverviewInformation(manga);
    }

    [RelayCommand]
    private void Copy(string text)
    {
        clipboardHelper.Copy(text);
    }

    [RelayCommand]
    public async Task OpenWith((Manga, string?) tuple)
    {
        Manga manga = default!;
        string way = "explorer.exe"; // 默认值
        switch (tuple)
        {
            case (Manga manga1, null):
                {
                    manga = manga1;
                    way = settingViewModel.AppConfig.MangaOpenWay3.DefaultWay;
                }
                break;
            case (Manga manga1, string way1):
                {
                    manga = manga1;
                    way = way1;
                }
                break;
        }

        try
        {
            await Process.Start(way, $"\"{manga.FilePath}\"").WaitForExitAsync();

            if (mangaFileIO.Exists(manga))
            {
                var path = await mangaFactory.GetCoverFile(manga);

                await mangaFileIO.LoadMangaInfo(manga);

                manga.CoverUri = path;

                // 本来这个是在后台线程中执行的，但是因为LoadMangaInfo方法中有UI线程的操作，所以会报错，因此改为在UI线程中执行
                // 但是不知道为什么，这个本来在xaml中执行会报错，在viewmodel中就不报错
                //this.DispatcherQueue.TryEnqueue(async () =>
                //{
                //    await mangaFileIO.LoadMangaInfo(manga);

                //    manga.CoverUri = path;

                //});
            }
            else
            {
                observableCollectionVM.RemoveManga(manga);
                observableCollectionVM.InvokeEvent_AfterDeleteMnagaSource(manga);
            }
        }
        catch (Exception)
        {
            var appNotification = new AppNotificationBuilder()
                .AddText($"{manga.Name}\r{StringsExtension.ResourceLoader.GetString("OpenFailed")}")
                .BuildNotification();
            AppNotificationManager.Default.Show(appNotification);
        }
    }

    public async Task Reload(Manga manga) { }

    [RelayCommand]
    public async Task Delete(Manga manga)
    {
        try
        {
            var result = await dialogHelper.ConfirmDeleteSourceFileDialog(manga);
            if (result)
            {
                observableCollectionVM.RemoveManga(manga);
                observableCollectionVM.InvokeEvent_AfterDeleteMnagaSource(manga);
            }
        }
        catch (UnauthorizedAccessException)
        {
            observableCollectionVM.AccessDenied();
        }
        catch (System.IO.IOException)
        {
            observableCollectionVM.AccessDenied();
        }
    }

    public async Task MoveManga(Manga manga, MangasGroup way)
    {
        try
        {
            string newpath = await Task.Run(() =>
                mangaFileIO.MoveManga(manga, way.FolderPath, null)
            );
            manga.FilePath = newpath;

            observableCollectionVM.PlaceInCorrectGroup(manga);
        }
        catch (UnauthorizedAccessException)
        {
            observableCollectionVM.AccessDenied();
        }
        catch (System.IO.IOException)
        {
            observableCollectionVM.AccessDenied();
        }
    }

    [RelayCommand]
    private async Task ExportAsPDF(Manga manga)
    {
        await storageOperation.ExportAsPDFAsync(manga);
    }

    [RelayCommand]
    private void LocateMangaInFolder(Manga manga)
    {
        ExplorerFile.ExplorerSelectFile(manga.FilePath);
    }

    [RelayCommand]
    private async Task RenameManga(Manga manga)
    {
        await dialogHelper.RenameSourceFileInDialog(manga);
    }
}
