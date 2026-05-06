// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using EroMangaManager.WinUI3.UserControls;

namespace EroMangaManager.WinUI3.Views.FunctionChildPages;

/// <summary>
/// 不再使用，使用第2版
/// </summary>
public sealed partial class RemoveRepeatTags : Page
{
    private ObservableCollection<Manga> RepaetBooks { get; } = [];

    /// <summary>
    ///
    /// </summary>
    public RemoveRepeatTags()
    {
        InitializeComponent();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="e"></param>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        foreach (var book in App.Current.GlobalViewModel.MangaList)
        {
            if (book.Tags.ContainRepeat())
            {
                RepaetBooks.Add(book);
            }
        }
    }

    private void SingleMangaRename_New(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;
        var stackpanel = button.Parent as StackPanel;

        var book = button.DataContext as Manga;

        var control = stackpanel.FindName("newnameBox") as TextBox;
        var text = control.Text;
        TrySetNewName(book, text);
        RemoveIfTagRepeat(book);
    }

    private void TrySetNewName(Manga book, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        else
        {
            try
            {
                // TODO 重命名可能存在bug，如重复名称
                string oldname = book.FilePath;
                string newname = Path.Combine(Path.GetDirectoryName(oldname), text + ".zip");
                System.IO.File.Move(oldname, newname);
                book.FilePath = Path.Combine(book.FolderPath, text + ".zip");
            }
            catch { }
        }
    }

    private void RemoveIfTagRepeat(Manga book)
    {
        if (!book.Tags.ContainRepeat())
        {
            RepaetBooks.Remove(book);
        }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        var button = sender as Button;

        var book = button.DataContext as Manga;

        RepaetBooks.Remove(book);
    }

    private void TagListOrder_Loaded(object sender, RoutedEventArgs e)
    {
        var order = sender as TagListOrder;
        var manga = order.DataContext as Manga;
        var items = BracketBasedStringParser.SplitByBrackets_KeepBracket(manga.FileDisplayName);
        order.Sources = items;
    }

    private void NewnameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        var textbox = sender as TextBox;
        var book = textbox.DataContext as Manga;

        var text = textbox.Text;
        //TODO 这有严重bug，每次文字切换，会直接改名
        TrySetNewName(book, text);
        RemoveIfTagRepeat(book);
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        //var control = sender as UserControl;
        //var newnamebox = control.FindName("newnamebox");
    }
}