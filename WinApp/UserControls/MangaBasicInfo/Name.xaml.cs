// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class Name : UserControl
{
    // ViewModel 用于在 XAML 中绑定，必须为 DependencyProperty
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(MangaOperationViewModel),
        typeof(Name),
        new PropertyMetadata(null)
    );

    public MangaOperationViewModel ViewModel
    {
        get => (MangaOperationViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }
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

    public Name()
    {
        InitializeComponent();
    }

    // 4. 跳转到全局搜索页面
    [RelayCommand]
    private async Task Navigate(string text)
    {
        if (string.IsNullOrEmpty(text))
            return;

        var mainpage = App.Services.GetRequiredService<MainPage>();
        await mainpage.NavigateToPage<GlobalSearchPage>().Search(text);

        //MainPage.Current?.MainFrame.Navigate(typeof(GlobalSearchPage) , text);
    }
}
