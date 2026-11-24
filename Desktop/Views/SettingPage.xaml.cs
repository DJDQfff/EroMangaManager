// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板
using Windows.System; // 需要引入此命名空间
namespace Desktop.Views;

/// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
public sealed partial class SettingPage : Page
{
    public SettingViewModel ViewModel { private set; get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    public SettingPage(SettingViewModel viewModel, CommonSettingPage commonSettingPage)
    {
        InitializeComponent();

        SettingContainer.Content = commonSettingPage;
        ViewModel = viewModel;
    }



    private async void OpenStore_Click(object sender, RoutedEventArgs e)
    {
        // 使用你截图中的 ProductId
        var uri = new Uri("ms-windows-store://pdp/?ProductId=9N7C8ZDQ1TJ8");
        await Launcher.LaunchUriAsync(uri);
    }
}