namespace WinApp.UserControls;

public sealed partial class RenameMangaByEditString : UserControl
{
    readonly IServiceProvider services = App.Services;

    public RenameMangaByEditString()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty MangaProperty = DependencyProperty.Register(
        nameof(Manga),
        typeof(Manga),
        typeof(RenameMangaByEditString),
        new PropertyMetadata(null, OnMangaChanged)
    );
    public Manga Manga
    {
        get => (Manga)GetValue(MangaProperty);
        set => SetValue(MangaProperty, value);
    }

    private static void OnMangaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RenameMangaByEditString control && e.NewValue is Manga newmanga)
        {
            control.DataContext = newmanga;
            control.textbox.Text = newmanga.FileDisplayName;
        }
    }

    public bool IsNewnameOK
    {
        set;
        get { return field && (NewDisplayName != Manga?.FileDisplayName); }
    }
    public event Action WrongInput = delegate { };
    public event Action CorrectInput = delegate { };
    public string NewDisplayName => textbox.Text;

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NewDisplayName))
        {
            hinttextblock.Text = StringsExtension.ResourceLoader.GetString("DontUseEmptyString");
            IsNewnameOK = false;
            RenameButton.IsEnabled = false;
            WrongInput?.Invoke();
        }
        else if (NewDisplayName.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
        {
            hinttextblock.Text = StringsExtension.ResourceLoader.GetString("ContainInvalaidChar");
            IsNewnameOK = false;
            RenameButton.IsEnabled = false;
            WrongInput?.Invoke();
        }
        else
        {
            hinttextblock.Text = string.Empty;
            IsNewnameOK = true;
            RenameButton.IsEnabled = true;
            CorrectInput?.Invoke();
        }
    }

    [RelayCommand]
    private async Task Rename()
    {
        var newname = NewDisplayName;
        try
        {
            var manga = Manga;
            string newpath = await Task.Run(() =>
                services.GetRequiredService<MangaFileIO>().MoveManga(manga, null, newname)
            );
            manga?.FilePath = newpath;
        }
        catch (UnauthorizedAccessException)
        {
            services.GetRequiredService<ObservableCollectionVM>().AccessDenied();
        }
        catch (System.IO.IOException)
        {
            services.GetRequiredService<ObservableCollectionVM>().AccessDenied();
        }
    }
}
