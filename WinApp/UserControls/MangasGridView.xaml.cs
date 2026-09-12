// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls;

public sealed partial class MangasGridView : UserControl
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(object),
        typeof(MangasGridView),
        new PropertyMetadata(null)
    );
    public object ItemsSource
    {
        get => (object)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    [RelayCommand]
    public async Task NavigateSearchName(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        //await mainpage.NavigateToPage<GlobalSearchPage>().Search(text);
        var globalSearchPage =
            MainPage.NavigateToPage(StringsEnum.GlobalSearch) as GlobalSearchPage;
        await globalSearchPage!.Search(text);
        //MainPage.Current?.MainFrame.Navigate(typeof(GlobalSearchPage) , text);
    }

    [RelayCommand]
    private async Task NavigateSearchTags(string text)
    {
        var page = MainPage.NavigateToPage(StringsEnum.GlobalSearch) as GlobalSearchPage;
        await page!.Search(new string[] { text });
    }

    public MangasGridView()
    {
        InitializeComponent();
    }

    // 不需要 Register / GetValue / SetValue
    public ClipboardHelper ClipboardHelper { get; set; } = null!;
    public ContentDialogCreater ContentDialogCreater { get; set; } = null!;
    public MangaFactory MangaFactory { get; set; } = null!;
    public MangaIO MangaFileIO { get; set; } = null!;
    public CoverSetter CoverSetter { get; set; } = null!;
    public CoverHelper CoverHelper { get; set; } = null!;
    public MainPage MainPage { get; set; } = null!;
    public INotifier Notifier { get; set; } = null!;

    // 5. ObservableCollectionVM
    public static readonly DependencyProperty ObservableCollectionVMProperty =
        DependencyProperty.Register(
            nameof(ObservableCollectionVM),
            typeof(ObservableCollectionVM),
            typeof(MangasGridView),
            new PropertyMetadata(null)
        );

    public ObservableCollectionVM ObservableCollectionVM
    {
        get => (ObservableCollectionVM)GetValue(ObservableCollectionVMProperty);
        set => SetValue(ObservableCollectionVMProperty, value);
    }

    // 7. SettingViewModel
    public static readonly DependencyProperty SettingViewModelProperty =
        DependencyProperty.Register(
            nameof(SettingViewModel),
            typeof(SettingViewModel),
            typeof(MangasGridView),
            new PropertyMetadata(null)
        );

    public SettingViewModel SettingViewModel
    {
        get => (SettingViewModel)GetValue(SettingViewModelProperty);
        set => SetValue(SettingViewModelProperty, value);
    }

    /// <summary>
    /// TODO 考虑改为依赖属性，方便在xaml中绑定。
    /// TODO 考虑多个DataTemplate合并为一个，使用VisualStateManager切换不同的布局，减少代码量。
    /// </summary>
    public string ItemTemplateIndex
    {
        get => field ?? "0";
        set
        {
            if (value != null)
            {
                field = value;
                gridview.ItemTemplate = Resources[field] as DataTemplate;
            }
        }
    }

    private void Moveto_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga manga })
        {
            moveto.Items.Clear();
            var ways = ObservableCollectionVM.MangasGroups;
            foreach (var way in ways)
            {
                var item = new MenuFlyoutItem { Text = way.FolderPath };
                moveto.Items.Add(item);

                if (
                    way.FolderPath == manga.FolderPath
                    || string.Equals(
                        Path.GetPathRoot(way.FolderPath),
                        Path.GetPathRoot(manga.FilePath),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                {
                    item.IsEnabled = false;
                    continue;
                }
                item.Click += async (sender, e) =>
                {
                    string newpath = null;
                    try
                    {
                        newpath = await Task.Run(() =>
                            MangaFileIO.MoveManga(manga, way.FolderPath, null)
                        );
                        manga.FilePath = newpath;

                        ObservableCollectionVM.PlaceInCorrectGroup(manga);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        ObservableCollectionVM.AccessDenied(newpath);
                    }
                    catch (System.IO.IOException)
                    {
                        ObservableCollectionVM.AccessDenied(newpath);
                    }
                };
            }
        }
    }

    private void MenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga mnaga })
        {
            openwith.Items.Clear();
            var ways = SettingViewModel.ExePaths;

            foreach (var way in ways)
            {
                var item = new MenuFlyoutItem { Text = Path.GetFileNameWithoutExtension(way) };

                openwith.Items.Add(item);

                item.Click += async (sender, e) =>
                {
                    var manga = mnaga; // 获取datacontext，可能导致ui线程错误
                    await OpenWith((manga, way));
                };
            }
        }
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
                    way = SettingViewModel.AppConfig.MangaOpenWay3.DefaultWay;
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

            if (MangaFileIO.Exists(manga))
            {
                var path = await MangaFileIO.GetCoverFile(manga);

                await MangaFileIO.LoadMangaInfo(manga);

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
                ObservableCollectionVM.RemoveManga(manga);
                ObservableCollectionVM.InvokeEvent_AfterDeleteMnagaSource(manga);
            }
        }
        catch (Exception)
        {
            Notifier.Notify(
                $"{manga.Name}\r{StringsExtension.ResourceLoader.GetString("OpenFailed")}"
            );
        }
    }

    private async void Image_Loaded(object sender, RoutedEventArgs e)
    {
        //var menuFlyoutSubItem = sender as MenuFlyoutSubItem;
        //var mnaga = menuFlyoutSubItem.DataContext as Manga;

        //if (mnaga?.CoverUri == services.GetRequiredService<CoverHelper>().DefaultCoverUri)
        //{
        //    // TODO 能运行，但搞不明白
        //    // 这里不能用：var coverPath = await  task.run(()=> MangaFactory.GetCoverFile(Source));
        //    // 会线程冲突
        //    // AI解释如下：调用task.run后，没有回到ui线程。
        //    // await 之后是否回到 UI 线程，取决于当前 SynchronizationContext。
        //    // WPF 有 DispatcherSynchronizationContext，await 会自动回到 UI 线程。
        //    // WinUI 3 没有 SynchronizationContext，await 之后就在 Task.Run 结束的那个线程池线程上继续跑，不会自动回 UI 线程。
        //    // 所以你用 await Task.Run(...) 就相当于主动离开 UI 线程，再也没回来。这是 WinUI 3 和 WPF 的关键差异。
        //    // 但是我在别的地方也有调用task.run为什么没这类问题

        //    // 靠不住，都后台自动管理实在是靠不住，在xaml中取消此方法
        //    var coverPath = await services.GetRequiredService<MangaFactory>().GetCoverFile(mnaga);

        //    mnaga.CoverUri = coverPath;

        //}
    }

    private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is Grid { DataContext: Manga manga })
        {
            await OpenWith((manga, null));
        }
    }

    [RelayCommand]
    public async Task Delete(Manga manga)
    {
        try
        {
            var result = await ContentDialogCreater.ConfirmDeleteSourceFileDialog(manga);
            if (result)
            {
                ObservableCollectionVM.RemoveManga(manga);
                ObservableCollectionVM.InvokeEvent_AfterDeleteMnagaSource(manga);
            }
        }
        catch (UnauthorizedAccessException)
        {
            var a = StringsExtension.ResourceLoader.GetString("AccessDenied");
            ObservableCollectionVM.AccessDenied(a);
        }
        catch (System.IO.IOException)
        {
            var a = StringsExtension.ResourceLoader.GetString("AccessDenied");

            ObservableCollectionVM.AccessDenied(a);
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
        await ContentDialogCreater.RenameSourceFileInDialog(
            manga,
            MangaFileIO,
            ObservableCollectionVM
        );
    }
}
