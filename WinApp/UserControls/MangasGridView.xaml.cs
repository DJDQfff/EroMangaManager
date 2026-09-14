// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls;

public sealed partial class MangasGridView : UserControl
{
    public static readonly DependencyProperty MangaSourceProperty = DependencyProperty.Register(
        nameof(MangaSource),
        typeof(IEnumerable<Manga>), // ✅ 改为强类型
        typeof(MangasGridView),
        new PropertyMetadata(null)
    );

    public IEnumerable<Manga> MangaSource
    {
        get => (IEnumerable<Manga>)GetValue(MangaSourceProperty);
        set => SetValue(MangaSourceProperty, value);
    }

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
        get => field ?? "Template0";
        set
        {
            if (value != null)
            {
                field = value;
                gridview.ItemTemplate = Resources[field] as DataTemplate;
            }
        }
    }

    public static readonly DependencyProperty MangaOperationViewModelProperty =
        DependencyProperty.Register(
            nameof(MangaOperationViewModel),
            typeof(MangaOperationViewModel),
            typeof(MangasGridView),
            new PropertyMetadata(null)
        );
    public MangaOperationViewModel MangaOperationViewModel
    {
        get => (MangaOperationViewModel)GetValue(MangaOperationViewModelProperty);
        set => SetValue(MangaOperationViewModelProperty, value);
    }

    private void Moveto_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga manga })
        {
            moveto.Items.Clear();
            var ways = MangaOperationViewModel.MangasGroups;
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
                    await MangaOperationViewModel.MoveManga(manga, way);
                ;
            }
        }
    }

    private void MenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga mnaga })
        {
            openwith.Items.Clear();
            var ways = MangaOperationViewModel.ExePaths;

            foreach (var way in ways)
            {
                var item = new MenuFlyoutItem { Text = Path.GetFileNameWithoutExtension(way) };

                openwith.Items.Add(item);

                item.Click += async (sender, e) =>
                {
                    var manga = mnaga; // 获取datacontext，可能导致ui线程错误
                    await MangaOperationViewModel.OpenWith((manga, way));
                };
            }
        }
    }

    private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is Grid { DataContext: Manga manga })
        {
            await MangaOperationViewModel.OpenWith((manga, null));
        }
    }

    private void openwith_Loading(FrameworkElement sender, object args)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga mnaga } flytotitem)
        {
            flytotitem.Items.Clear();
            var ways = MangaOperationViewModel.ExePaths;

            foreach (var way in ways)
            {
                var item = new MenuFlyoutItem { Text = Path.GetFileNameWithoutExtension(way) };

                flytotitem.Items.Add(item);

                item.Click += async (sender, e) =>
                {
                    var manga = mnaga; // 获取datacontext，可能导致ui线程错误
                    await MangaOperationViewModel.OpenWith((manga, way));
                };
            }
        }
    }
}
