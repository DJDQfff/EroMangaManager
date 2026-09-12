// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace WinApp.Views.MainPageChildPages;

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
}
