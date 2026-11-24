// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UnoLibrary.UserControls;

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

    ////不能使用普通属性，千问说普通属性无法在 XAML 中使用{Binding}绑定。
    ////必须使用 DependencyProperty
    ////public CoverHelper CoverHelper { set; get; } = null!;
    //public CoverHelper CoverHelper
    //{
    //    get => (CoverHelper)GetValue(CoverHelperProperty);
    //    set => SetValue(CoverHelperProperty, value);
    //}

    //// ViewModel 用于在 XAML 中绑定，必须为 DependencyProperty
    //public static readonly DependencyProperty CoverHelperProperty = DependencyProperty.Register(
    //    nameof(CoverHelper),
    //    typeof(CoverHelper),
    //    typeof(Cover),
    //    new PropertyMetadata(null)
    //);

    public Cover()
    {
        InitializeComponent();
    }

    private async void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        try
        {
//  在win上，路径被识别成绝对路径。
//但是在linux上，如/home/xxx/xxx.jpg被识别成相对路径，在xaml绑定中会在前面补上运行时目录
// 手动加载流赋值
            using (var stream = File.OpenRead(Manga.CoverUri))
            {
                var bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
                image.Source = bitmapImage; // 这里的 image 是你 XAML 中的 x:Name
            }
        }
        catch
        {
            // 加载错误图标
            image.Source = new SvgImageSource(new Uri("ms-appx:///Assets/error.svg")); // CoverHelper.ErrorCoverImage;
        }
    }
}