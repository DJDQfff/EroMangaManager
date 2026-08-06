// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WinApp.Views.SettingPageChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class CommonSettingPage : Page
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(SettingViewModel),
        typeof(CommonSettingPage),
        new PropertyMetadata(null)
    );

    // 2. 包装属性
    public SettingViewModel ViewModel
    {
        get => (SettingViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// 一般设置页面
    /// </summary>
    public CommonSettingPage(SettingViewModel settingViewModel, DialogHelper dialogHelper)
    {
        InitializeComponent();
        ViewModel = settingViewModel;
        ManageMangaOpenWay3Setting.DialogHelper = dialogHelper;
    }
}
