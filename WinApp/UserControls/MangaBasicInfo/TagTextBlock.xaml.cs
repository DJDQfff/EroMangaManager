// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.UserControls.MangaBasicInfo;

public sealed partial class TagTextBlock : UserControl
{
    // ViewModel 用于在 XAML 中绑定，必须为 DependencyProperty
    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
        nameof(ViewModel),
        typeof(MangaOperationViewModel),
        typeof(TagTextBlock),
        new PropertyMetadata(null)
    );

    public MangaOperationViewModel ViewModel
    {
        get => (MangaOperationViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text),
        typeof(string),
        typeof(TagTextBlock),
        new PropertyMetadata(null)
    );

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public TagTextBlock()
    {
        InitializeComponent();
    }

    [RelayCommand]
    private async Task NavigatetoSearch(string text)
    {
        await App
            .Services.GetRequiredService<MainPage>()
            .NavigateToPage<GlobalSearchPage>()
            .Search(new string[] { text });
    }
}
