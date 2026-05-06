// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(newtitlebar);
        AppWindow.SetIcon(CoverHelper.DefaultCoverPath);
        Title = ResourceLoader.GetForViewIndependentUse().GetString("AppDisplayName");
        _ = MainWindowFrame.Navigate(typeof(MainPage));
    }
}