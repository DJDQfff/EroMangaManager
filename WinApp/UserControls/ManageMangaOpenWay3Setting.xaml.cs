// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using UnoLibrary.Services;

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

    public static readonly DependencyProperty ContentDialogCreaterProperty =
        DependencyProperty.Register(
            nameof(ContentDialogCreater),
            typeof(ContentDialogCreater),
            typeof(ManageMangaOpenWay3Setting),
            new PropertyMetadata(null)
        );

    // 2. 包装属性
    public ContentDialogCreater ContentDialogCreater
    {
        get => (ContentDialogCreater)GetValue(ContentDialogCreaterProperty);
        set => SetValue(ContentDialogCreaterProperty, value);
    }

    [RelayCommand]
    private async Task AddExe()
    {
        var file = await ContentDialogCreater.PickSingleFile("选择exe文件", ".exe");
        if (file is not null)
        {
            ViewModel.AddExePath(file.Path);
        }
    }
}
