// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class Name : UserControl
{
    // ViewModel 用于在 XAML 中绑定，必须为 DependencyProperty

    public static readonly DependencyProperty MangaProperty = DependencyProperty.Register(
        nameof(Manga),
        typeof(Manga),
        typeof(Name),
        new PropertyMetadata(null)
    );

    // 2. 包装属性
    public Manga Manga
    {
        get => (Manga)GetValue(MangaProperty);
        set => SetValue(MangaProperty, value);
    }
    public static readonly DependencyProperty CopyCommandProperty = DependencyProperty.Register(
        nameof(CopyCommand),
        typeof(ICommand),
        typeof(Name), // ← 注意：这里填你的控件类名
        new PropertyMetadata(null)
    );

    public ICommand CopyCommand
    {
        get => (ICommand)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }

    public Name()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.Register(
        nameof(NavigateCommand),
        typeof(ICommand),
        typeof(Name), // ← 注意：这里填你的控件类名
        new PropertyMetadata(null)
    );

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }
    // 4. 跳转到全局搜索页面
}
