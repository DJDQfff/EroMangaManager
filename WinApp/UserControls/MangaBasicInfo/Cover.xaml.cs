// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

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

    public MangaOperationViewModel ViewModel
    {
        get => (MangaOperationViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    // ViewModel 用于在 XAML 中绑定，必须为 DependencyProperty
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(MangaOperationViewModel),
        typeof(Cover),
        new PropertyMetadata(null)
    );

    public Cover()
    {
        InitializeComponent();
    }

    private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
    {
        image.Source = ViewModel.ErrorImage;
    }
}
