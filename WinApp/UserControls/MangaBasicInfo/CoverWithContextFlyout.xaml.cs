// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.UserControls.MangaBasicInfo;

public sealed partial class CoverWithContextFlyout : UserControl
{
    public Manga Source
    {
        get => DataContext as Manga;
        set => DataContext = value;
    }

    public CoverWithContextFlyout()
    {
        InitializeComponent();
        DataContextChanged += (a, b) => this.Bindings.Update();
    }

    private void UserControl_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        MangaCommands.Instance.OpenWithManga.Execute((Source, App.Current.AppConfig.AppConfig.MangaOpenWay3.DefaultWay));
    }

    private void Moveto_Loaded(object sender, RoutedEventArgs e)
    {
        moveto.Items.Clear();
        var ways = App.Current.GlobalViewModel.MangaFolders;
        foreach (var way in ways)
        {
            var item = new MenuFlyoutItem { Text = way.FolderPath };
            moveto.Items.Add(item);

            if (way.FolderPath == Source.FolderPath || Path.GetPathRoot(way.FolderPath).ToLower() != Path.GetPathRoot(Source.FilePath).ToLower())
            {
                item.IsEnabled = false;
                continue;
            }
            item.Click += async (sender, e) =>
            {
                try
                {
                    var manga = Source;
                    string newpath = await Task.Run(() => MangaFileIO.MoveManga(manga, way.FolderPath, null));
                    Source.FilePath = newpath;

                    App.Current.GlobalViewModel.PlaceInCorrectGroup(Source);
                }
                catch (UnauthorizedAccessException) { App.Current.GlobalViewModel.AccessDenied(); }
                catch (System.IO.IOException ex)
                {
                    App.Current.GlobalViewModel.AccessDenied();
                }
            };
        }
    }

    private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        image.Source = CoverHelper.ErrorCoverImage;
    }

    private void MenuFlyoutItem_Loaded(object sender, RoutedEventArgs e)
    {
        openwith.Items.Clear();
        var ways = App.Current.AppConfig.ExePaths;




        foreach (var way in ways)
        {
            var item = new MenuFlyoutItem { Text = Path.GetFileNameWithoutExtension(way) };

            openwith.Items.Add(item);

            item.Click += (sender, e) =>
            {
                var manga = Source;// 获取datacontext，可能导致ui线程错误
                MangaCommands.Instance.OpenWithManga.Execute((manga, way));
            };
        }

    }
}