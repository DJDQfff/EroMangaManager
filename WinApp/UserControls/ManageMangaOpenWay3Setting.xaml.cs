// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls;

public sealed partial class ManageMangaOpenWay3Setting : UserControl
{
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(SettingViewModel),
        typeof(ManageMangaOpenWay3Setting),
        new PropertyMetadata(null)
    );

    // 2. 包装属性
    public SettingViewModel ViewModel
    {
        get => (SettingViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public ManageMangaOpenWay3Setting()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty DialogHelperProperty = DependencyProperty.Register(
        nameof(DialogHelper),
        typeof(DialogHelper),
        typeof(ManageMangaOpenWay3Setting),
        new PropertyMetadata(null)
    );

    // 2. 包装属性
    public DialogHelper DialogHelper
    {
        get => (DialogHelper)GetValue(DialogHelperProperty);
        set => SetValue(DialogHelperProperty, value);
    }

    [RelayCommand]
    private async Task AddExe()
    {
        var file = await DialogHelper.PickSingleFile("选择exe文件", ".exe");
        if (file is not null)
        {
            ViewModel.AddExePath(file.Path);
        }
    }
}
