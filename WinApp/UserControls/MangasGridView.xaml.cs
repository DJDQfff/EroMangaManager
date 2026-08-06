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

    public MangaOperationViewModel ViewModel { get; set; } =
        App.Services.GetRequiredService<MangaOperationViewModel>();

    public MangasGridView()
    {
        InitializeComponent();
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

    [RelayCommand]
    private async Task SearchSimilar(Manga manga)
    {
        await App
            .Services.GetRequiredService<MainPage>()
            .NavigateToPage<GlobalSearchPage>()
            .Search(manga.Name);
    }

    private void Moveto_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga manga })
        {
            moveto.Items.Clear();
            var ways = ViewModel.MangasGroups;
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
                    await ViewModel.MoveManga(manga, way);
                };
            }
        }
    }

    private void MenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga mnaga })
        {
            openwith.Items.Clear();
            var ways = ViewModel.ExePaths;

            foreach (var way in ways)
            {
                var item = new MenuFlyoutItem { Text = Path.GetFileNameWithoutExtension(way) };

                openwith.Items.Add(item);

                item.Click += async (sender, e) =>
                {
                    var manga = mnaga; // 获取datacontext，可能导致ui线程错误
                    await ViewModel.OpenWith((manga, way));
                };
            }
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
            await ViewModel.OpenWith((manga, null));
        }
    }
}
