namespace WinApp.UserControls;

public sealed partial class RenameMangaByDragTag : UserControl
{
    readonly IServiceProvider services = App.Services;

    public static readonly DependencyProperty MangaProperty = DependencyProperty.Register(
        nameof(Manga),
        typeof(Manga),
        typeof(RenameMangaByDragTag),
        new PropertyMetadata(null, OnMangaChanged)
    );

    public RenameMangaByDragTag()
    {
        InitializeComponent();
    }

    // 2. �����İ�װ����
    public Manga Manga
    {
        get => (Manga)GetValue(MangaProperty);
        set => SetValue(MangaProperty, value);
    }

    private static void OnMangaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (RenameMangaByDragTag)d;
        var newManga = e.NewValue as Manga;

        control.DataContext = newManga;

        if (newManga != null)
        {
            control.order.Sources = BracketBasedStringParser.SplitByBrackets_KeepBracket(
                newManga.FileDisplayName
            );
        }
    }

    public event Action<Manga> NameChanged = delegate { };

    private async void SingleMangaRename_New(object sender, RoutedEventArgs e)
    {
        var manga = Manga;
        if (manga == null)
            return; // ��ֹ������

        var text = newnameBox.Text; // ��ǰ�� UI �߳�ȡ���ı�

        try
        {
            string newpath = await Task.Run(() =>
                services.GetRequiredService<MangaFileIO>().MoveManga(manga, null, text)
            );
            manga.FilePath = newpath;
        }
        catch (UnauthorizedAccessException)
        {
            services.GetRequiredService<ObservableCollectionVM>().AccessDenied();
        }
        catch (System.IO.IOException)
        {
            services.GetRequiredService<ObservableCollectionVM>().AccessDenied();
        }

        NameChanged?.Invoke(manga);
    }
}
