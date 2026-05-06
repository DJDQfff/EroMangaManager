// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class IrregularNameSearch : Page
    {
        private readonly ObservableCollection<Manga> books = [];

        public IrregularNameSearch()
        {
            InitializeComponent();
            App.Current.GlobalViewModel.EventAfterDeleteMangaSource += Delete;
        }
        private void Delete(Manga manga)
        {
            _ = books.Remove(manga);
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            books.Clear();
            foreach (var book in App.Current.GlobalViewModel.MangaList)
            {
                if (
                       (checkbox0.IsChecked == true && BracketBasedStringParser.CorrectBracketPairConut(book.FileDisplayName) == -1)
                    || (checkbox1.IsChecked == true && !BracketBasedStringParser.ContainAnyBrackets(book.FileDisplayName))
                    || (checkbox2.IsChecked == true && BracketBasedStringParser.Get_OutsideContent(book.FileDisplayName).Count == 0)
                    || (checkbox3.IsChecked == true && book.Tags.ContainRepeat())
                    || (!string.IsNullOrEmpty(textbox.Text) && book.MangaName.Contains(textbox.Text))
                )
                {
                    books.Add(book);
                }
            }
            await App.Current.CoverSetter.AddWork(books);
        }
    }
}