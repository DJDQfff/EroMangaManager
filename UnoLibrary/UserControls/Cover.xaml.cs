// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using UnoLibrary.Services;

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class Cover : UserControl
{
    public static readonly DependencyProperty MangaProperty = DependencyProperty.Register(
        nameof(Manga),
        typeof(Manga),
        typeof(Cover),
        new PropertyMetadata(null)
    );

    // 2. 包装属性
    public Manga Manga
    {
        get => (Manga)GetValue(MangaProperty);
        set => SetValue(MangaProperty, value);
    }

    //不能使用普通属性，千问说普通属性无法在 XAML 中使用{Binding}绑定。
    //必须使用 DependencyProperty
    //public CoverHelper CoverHelper { set; get; } = null!;
    public CoverHelper CoverHelper
    {
        get => (CoverHelper)GetValue(CoverHelperProperty);
        set => SetValue(CoverHelperProperty, value);
    }

    // ViewModel 用于在 XAML 中绑定，必须为 DependencyProperty
    public static readonly DependencyProperty CoverHelperProperty = DependencyProperty.Register(
        nameof(CoverHelper),
        typeof(CoverHelper),
        typeof(Cover),
        new PropertyMetadata(null)
    );

    public Cover()
    {
        InitializeComponent();
    }

    private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        image.Source = CoverHelper.ErrorCoverImage;
    }
}
