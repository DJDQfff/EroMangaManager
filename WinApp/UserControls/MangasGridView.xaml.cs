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

    /// TODO 考虑多个DataTemplate合并为一个，使用VisualStateManager切换不同的布局，减少代码量。
    /// <summary>
    /// TODO 考虑改为依赖属性，方便在xaml中绑定。改了之后奇怪bug
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
        set
        {
            SetValue(MangaOperationViewModelProperty, value);
            MangaOperationViewModel.ExternalProcessExited += manga => { };
        }
    }

    private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is Grid { DataContext: Manga manga })
        {
            await MangaOperationViewModel.OpenWith((manga, null));
            if (MangaOperationViewModel.MangaIO.Exists(manga))
            {
                var path = await MangaOperationViewModel.MangaIO.GetCoverFile(manga);

                await MangaOperationViewModel.MangaIO.LoadMangaInfo(manga);

                manga.CoverUri = path;
            }
            else
            {
                MangaOperationViewModel.ObservableCollectionVM.RemoveManga(manga);
                MangaOperationViewModel.ObservableCollectionVM.InvokeEvent_AfterDeleteMnagaSource(
                    manga
                );
            }
        }
    }

    private void openwith_Loading(FrameworkElement sender, object args)
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

    /// <summary>
    /// 不要在xaml里使用绑定，坑死人
    /// template2在findsamemangapage里使用时
    /// 可以执行事件但无法执行command
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuFlyout_Opening(object sender, object e)
    {
        if (sender is MenuFlyout { Target: FrameworkElement { DataContext: Manga manga } })
        {
            LocateItem.Command = MangaOperationViewModel.LocateMangaInFolderCommand;
            LocateItem.CommandParameter = manga;

            OverviewInformation.Command = MangaOperationViewModel
                .ContentDialogCreater
                .OverviewInformationCommand;
            OverviewInformation.CommandParameter = manga;

            Rename.Command = MangaOperationViewModel.RenameMangaCommand;
            Rename.CommandParameter = manga;

            Export.Command = MangaOperationViewModel.ContentDialogCreater.ExportAsPDFCommand;
            Export.CommandParameter = manga;

            Delete.Command = MangaOperationViewModel.DeleteCommand;
            Delete.CommandParameter = manga;
        }
    }

    private void moveto_Loading(FrameworkElement sender, object args)
    {
        if (sender is MenuFlyoutSubItem { DataContext: Manga manga })
        {
            moveto.Items.Clear();
            var ways = MangaOperationViewModel.MangasGroups;
            foreach (var way in ways)
            {
                var item = new MenuFlyoutItem { Text = way.FolderPath };
                moveto.Items.Add(item);

                if (way.FolderPath == manga.FolderPath) //不能同文件夹移动
                //|| !string.Equals(
                //    Path.GetPathRoot(way.FolderPath),
                //    Path.GetPathRoot(manga.FilePath),
                //    StringComparison.OrdinalIgnoreCase
                //不能跨盘移动（没做这个功能）

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
}
