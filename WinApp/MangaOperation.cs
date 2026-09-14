namespace WinApp;

public partial class MangaOperationViewModel(
    CoverHelper coverHelper,
    ClipboardHelper clipboardHelper,
    ObservableCollectionVM observableCollectionVM,
    ContentDialogCreater contentDialogCreater,
    MangaFactory mangaFactory,
    MainPage mainPage,
    MangaIO mangaIO,
    SettingViewModel settingViewModel,
    INotifier notifier
)
{
    public ICommand ClipboardHelperCopyCommand => clipboardHelper.CopyCommand;
    public ContentDialogCreater ContentDialogCreater => contentDialogCreater;
    public ObservableCollection<string> ExePaths => settingViewModel.ExePaths;
    public SvgImageSource ErrorImage => coverHelper.ErrorCoverImage;
    public ObservableCollection<MangasGroup> MangasGroups => observableCollectionVM.MangasGroups;

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
    private async Task NavigateSearchTags(string text)
    {
        var page = mainPage.NavigateToPage(StringsEnum.GlobalSearch) as GlobalSearchPage;
        await page!.Search(new string[] { text });
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

            if (mangaIO.Exists(manga))
            {
                var path = await mangaIO.GetCoverFile(manga);

                await mangaIO.LoadMangaInfo(manga);

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
            notifier.Notify(
                $"{manga.Name}\r{StringsExtension.ResourceLoader.GetString("OpenFailed")}"
            );
        }
    }

    public async Task Reload(Manga manga) { }

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
    private void LocateMangaInFolder(Manga manga)
    {
        ExplorerFile.ExplorerSelectFile(manga.FilePath);
    }

    [RelayCommand]
    private async Task RenameManga(Manga manga)
    {
        await contentDialogCreater.RenameSourceFileInDialog(manga, mangaIO);
    }
}
