// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class TagsItemRepeater : UserControl
{
    // 将 ViewModel 改为依赖属性
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(MangaOperationViewModel),
        typeof(TagsItemRepeater),
        new PropertyMetadata(null)
    );

    public MangaOperationViewModel ViewModel
    {
        get => (MangaOperationViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public TagsItemRepeater()
    {
        InitializeComponent();
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
