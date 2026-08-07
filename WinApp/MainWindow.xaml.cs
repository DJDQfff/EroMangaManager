// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using UnoLibrary.Services;

namespace UnoLibrary;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainWindow(CoverHelper coverHelper)
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(newtitlebar);
        AppWindow.SetIcon(coverHelper.DefaultCoverUri);
    }

    public void SetPage<TPage>(TPage page)
        where TPage : Page
    {
        // 1. 清空容器，移除旧的页面
        PageContainer.Content = null;

        // 2. 将新页面放入容器
        PageContainer.Content = page;
    }
}
