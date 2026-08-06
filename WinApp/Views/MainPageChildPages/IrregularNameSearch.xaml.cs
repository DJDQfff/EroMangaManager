// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class IrregularNameSearch : Page
{
    private readonly ObservableCollection<Manga> books = [];
    readonly ObservableCollectionVM observableCollectionVM;
    readonly CoverSetter coverSetter;

    public IrregularNameSearch(
        ObservableCollectionVM _observableCollectionVM,
        CoverSetter _coverSetter
    )
    {
        InitializeComponent();
        observableCollectionVM = _observableCollectionVM;
        this.coverSetter = _coverSetter;
        observableCollectionVM.EventAfterDeleteMangaSource += Delete;
    }

    private void Delete(Manga manga)
    {
        _ = books.Remove(manga);
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        books.Clear();
        foreach (var book in observableCollectionVM.MangaList)
        {
            if (
                (
                    checkbox0.IsChecked == true
                    && BracketBasedStringParser.CorrectBracketPairConut(book.FileDisplayName) == -1
                )
                || (
                    checkbox1.IsChecked == true
                    && !BracketBasedStringParser.ContainAnyBrackets(book.FileDisplayName)
                )
                || (
                    checkbox2.IsChecked == true
                    && BracketBasedStringParser.Get_OutsideContent(book.FileDisplayName).Count == 0
                )
                || (checkbox3.IsChecked == true && book.Tags.ContainRepeat())
                || (!string.IsNullOrEmpty(textbox.Text) && book.Name.Contains(textbox.Text))
            )
            {
                books.Add(book);
            }
        }
        await coverSetter.MultiLoadWork(books, true, true);
    }
}
