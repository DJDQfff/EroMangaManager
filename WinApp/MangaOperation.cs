using Microsoft.VisualBasic;

namespace WinApp.Services;

public partial class MangaOperationViewModel(
    CoverHelper coverHelper,
    ClipboardHelper clipboardHelper,
    ObservableCollectionVM observableCollectionVM,
    ContentDialogCreater contentDialogCreater,
    MainPage mainPage,
    MangaIO mangaIO,
    SettingViewModel settingViewModel,
    INotifier notifier
)
{
    public MangaIO MangaIO => mangaIO;
    public ObservableCollectionVM ObservableCollectionVM => observableCollectionVM;
    public ICommand ClipboardHelperCopyCommand => clipboardHelper.CopyCommand;
    public ContentDialogCreater ContentDialogCreater => contentDialogCreater;
    public ObservableCollection<string> ExePaths => settingViewModel.ExePaths;
    public SvgImageSource ErrorImage => coverHelper.ErrorCoverImage;
    public ObservableCollection<MangasGroup> MangasGroups => observableCollectionVM.MangasGroups;
    public event Action<Manga> ExternalProcessExited;

    [RelayCommand]
    public async Task NavigateSearchName(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        //await mainpage.NavigateToPage<GlobalSearchPage>().Search(text);
        var globalSearchPage =
            mainPage.NavigateToPage(StringsEnum.GlobalSearch) as GlobalSearchPage;
        await globalSearchPage!.Search(text);
        //MainPage.Current?.MainFrame.Navigate(typeof(GlobalSearchPage) , text);
    }

    [RelayCommand]
    public async Task NavigateSearchTags(string text)
    {
        var page = mainPage.NavigateToPage(StringsEnum.GlobalSearch) as GlobalSearchPage;
        await page!.Search(new string[] { text });
    }

    [RelayCommand]
    public async Task OpenWith((Manga, string?) tuple)
    {
        Manga manga = default!;
        string way = settingViewModel.AppConfig.MangaOpenWay3.DefaultWay;

        // 简化 switch 逻辑
        if (tuple is (Manga m, string w))
        {
            manga = m;
            way = w;
        }
        else if (tuple is (Manga m1, null))
        {
            manga = m1;
        }

        try
        {
            await Process.Start(way, $"\"{manga.FilePath}\"").WaitForExitAsync();
            ExternalProcessExited?.Invoke(manga);
            return;
        }
        catch (Exception)
        {
            notifier.Notify(
                $"{manga.Name}\r{StringsExtension.ResourceLoader.GetString("OpenFailed")}"
            );
        }
    }

    [RelayCommand]
    public async Task Delete(Manga manga)
    {
        try
        {
            var result = await contentDialogCreater.ConfirmDeleteSourceFileDialog(manga);
            if (result)
            {
                observableCollectionVM.RemoveManga(manga);
                observableCollectionVM.InvokeEvent_AfterDeleteMnagaSource(manga);
            }
        }
        catch (UnauthorizedAccessException)
        {
            notifier.NotifyAccessDenied(manga.Name);
        }
        catch (System.IO.IOException)
        {
            notifier.NotifyAccessDenied(manga.Name);
        }
    }

    public async Task MoveManga(Manga manga, MangasGroup way)
    {
        try
        {
            string newpath = await Task.Run(() => mangaIO.MoveManga(manga, way.FolderPath, null));
            manga.FilePath = newpath;

            observableCollectionVM.PlaceInCorrectGroup(manga);
        }
        catch (UnauthorizedAccessException)
        {
            notifier.NotifyAccessDenied(manga.Name);
        }
        catch (System.IO.IOException)
        {
            notifier.NotifyAccessDenied(manga.Name);
        }
    }

    [RelayCommand]
    public void LocateMangaInFolder(Manga manga) => ExplorerFile.ExplorerSelectFile(manga.FilePath);

    [RelayCommand]
    public async Task RenameManga(Manga manga)
    {
        await contentDialogCreater.RenameSourceFileInDialog(manga, mangaIO);
    }
}
