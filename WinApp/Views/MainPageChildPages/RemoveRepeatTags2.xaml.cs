// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.Views.MainPageChildPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RemoveRepeatTags2 : Page
{
    private ObservableCollection<Manga> RepaetBooks { get; } = [];

    public RemoveRepeatTags2()
    {
        InitializeComponent();
        foreach (var book in App.Current.GlobalViewModel.MangaList)
        {
            if (book.Tags.ContainRepeat())
            {
                RepaetBooks.Add(book);
            }
        }
    }

    private void RemoveIfTagRepeat(Manga book)
    {
        if (!book.Tags.ContainRepeat())
        {
            _ = RepaetBooks.Remove(book);
        }
    }
}