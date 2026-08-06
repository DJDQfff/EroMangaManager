namespace UnoLibrary;

public sealed partial class SizeImagesChaptersAmount : UserControl
{
    public SizeImagesChaptersAmount()
    {
        InitializeComponent();
    }

    // 1. 定义依赖属性 (注意：属性名必须是 Manga，这样才能在 XAML 里用 Manga.FileSize)
    public static readonly DependencyProperty MangaProperty = DependencyProperty.Register(
        nameof(Manga), // 属性名称
        typeof(Manga), // 属性类型
        typeof(SizeImagesChaptersAmount), // 所属控件类型
        new PropertyMetadata(null)
    ); // 默认值

    // 2. 包装属性
    public Manga Manga
    {
        get => (Manga)GetValue(MangaProperty);
        set => SetValue(MangaProperty, value);
    }
}
