using Desktop.Views;

namespace Desktop;

public partial class MangaOperationViewModel(
    CoverHelper coverHelper,
    ClipboardHelper clipboardHelper,
    ObservableCollectionVM observableCollectionVm,
    ContentDialogCreater contentDialogCreater,
    MainPage mainPage,
    MangaIO mangaIO,
    SettingViewModel settingViewModel,
    INotifier notifier
)
{
    public MangaIO MangaIO => mangaIO;
    public ObservableCollectionVM ObservableCollectionVM => observableCollectionVm;
    public ICommand ClipboardHelperCopyCommand => clipboardHelper.CopyCommand;
    public ContentDialogCreater ContentDialogCreater => contentDialogCreater;
    public ObservableCollection<string> ExePaths => settingViewModel.ExePaths;
    public SvgImageSource ErrorImage => coverHelper.ErrorCoverImage;
    public ObservableCollection<MangasGroup> MangasGroups => observableCollectionVm.MangasGroups;


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
    public void OpenWith((Manga, string?) tuple)
    {
        Manga manga = null!;
        string? way = settingViewModel.AppConfig.MangaOpenWay3.DefaultWay;

        if (tuple is (Manga m, string w))
        {
            manga = m;
            way = w;
        }
        else if (tuple is (Manga m1, null))
        {
            manga = m1;
        }

        Process? process = null;
        try
        {
            if (string.IsNullOrWhiteSpace(way))
            {
                // 这里如果 FileLauncher 能返回 Process 最好，但如果是系统默认打开，通常拿不到
                FileLauncher.OpenWithSystemDefaultApp(manga.FilePath);
            }
            else
            {
                // 推荐给 FileLauncher.OpenWithSpecifiedApp 稍微改一下，让它返回 Process 对象
                process = FileLauncher.OpenWithSpecifiedAppAndReturn(way, manga.FilePath);
            }
        }
        catch (Exception)
        {
            notifier.Notify($"{manga.Name}\r{StringsExtension.ResourceLoader.GetString("OpenFailed")}");
            return;
        }

        // --- 核心改造：统一在这里处理重载逻辑 ---
        if (process != null)
        {
            // 指定了程序：监听进程退出
            process.EnableRaisingEvents = true;
            process.Exited += async (s, e) =>
            {
                // 回到 UI 线程处理更新
                await OnExternalProcessExited(manga);
            };
        }
        else
        {
            // 系统默认打开（拿不到 Process）：启动后延时几秒自动重载一次
            // 因为系统默认程序不确定，无法监听退出，只能盲猜一个时间
            _ = Task.Delay(3000).ContinueWith(_ => 
            {
                // 确保在 UI 线程执行
                return OnExternalProcessExited(manga);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    // 统一处理重载和移除逻辑
    private async Task OnExternalProcessExited(Manga manga)
    {
        if (MangaIO.Exists(manga))
        {
            var path = await MangaIO.GetCoverFile(manga);
            await MangaIO.LoadMangaInfo(manga);
            
            // 这里注意：如果 Manga 实现了 INotifyPropertyChanged，直接赋值即可更新 UI
            manga.CoverUri = path; 
            
            // 如果有必要，刷新 ObservableCollection 以触发 UI 更新
            // observableCollectionVm.PlaceInCorrectGroup(manga); 
        }
        else
        {
            observableCollectionVm.RemoveManga(manga);
            observableCollectionVm.InvokeEvent_AfterDeleteMnagaSource(manga);
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
                observableCollectionVm.RemoveManga(manga);
                observableCollectionVm.InvokeEvent_AfterDeleteMnagaSource(manga);
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

            observableCollectionVm.PlaceInCorrectGroup(manga);
            observableCollectionVm.InvokeEvent_AfterMoveMnagaSource(manga);
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
    public void LocateMangaInFolder(Manga manga) => FileLauncher.LocateInFileManager(manga.FilePath);

    [RelayCommand]
    public async Task RenameManga(Manga manga)
    {
        await contentDialogCreater.RenameSourceFileInDialog(manga, mangaIO);
    }
}