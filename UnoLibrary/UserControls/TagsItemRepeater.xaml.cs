// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class TagsItemRepeater : UserControl
{
    public TagsItemRepeater()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty CopyCommandProperty = DependencyProperty.Register(
        nameof(CopyCommand),
        typeof(ICommand),
        typeof(TagsItemRepeater), // ← 注意：这里填你的控件类名
        new PropertyMetadata(null)
    );

    public ICommand CopyCommand
    {
        get => (ICommand)GetValue(CopyCommandProperty);
        set => SetValue(CopyCommandProperty, value);
    }
    public static readonly DependencyProperty NavigateCommandProperty = DependencyProperty.Register(
        nameof(NavigateCommand),
        typeof(ICommand),
        typeof(TagsItemRepeater), // ← 注意：这里填你的控件类名
        new PropertyMetadata(null)
    );

    public ICommand NavigateCommand
    {
        get => (ICommand)GetValue(NavigateCommandProperty);
        set => SetValue(NavigateCommandProperty, value);
    }

    public static readonly DependencyProperty MangaProperty = DependencyProperty.Register(
        nameof(Manga),
        typeof(Manga),
        typeof(TagsItemRepeater),
        new PropertyMetadata(null)
    );

    // 2. 包装属性
    public Manga Manga
    {
        get => (Manga)GetValue(MangaProperty);
        set => SetValue(MangaProperty, value);
    }
}
