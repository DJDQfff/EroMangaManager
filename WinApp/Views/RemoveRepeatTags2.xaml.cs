// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RemoveRepeatTags2 : Page
{
    private ObservableCollection<Manga> RepaetBooks { get; } = [];

    public RemoveRepeatTags2()
    {
        InitializeComponent();
        foreach (var book in App.Services.GetRequiredService<ObservableCollectionVM>().MangaList)
        {
            if (book.Tags.Distinct().Count() < book.Tags.Length/*去重后变少了，存在重复内容*/)
            {
                RepaetBooks.Add(book);
            }
        }
    }

    private void RemoveIfTagRepeat(Manga book)
    {
        if (!(book.Tags.Distinct().Count() < book.Tags.Length/*去重后变少了，存在重复内容*/))
        {
            _ = RepaetBooks.Remove(book);
        }
    }
}
