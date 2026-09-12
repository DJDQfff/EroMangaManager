// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using WinApp.UserControls;

namespace WinApp.Views.FunctionChildPages;

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

        foreach (var book in App.Services.GetRequiredService<ObservableCollectionVM>().MangaList)
        {
            if (book.Tags.ContainRepeat())
            {
                RepaetBooks.Add(book);
            }
        }
    }

    private void SingleMangaRename_New(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { DataContext: Manga book, Parent: StackPanel stackpanel })
        {
            return;
        }
        if (stackpanel.FindName("newnameBox") is TextBox control)
        {
            var text = control.Text;
            TrySetNewName(book, text);
            RemoveIfTagRepeat(book);
        }
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
                string? directory = Path.GetDirectoryName(oldname);
                // ✅ 使用 ThrowIfNull，编译器能完美追踪流分析！
                ArgumentNullException.ThrowIfNull(directory, $"无法从路径 '{oldname}' 中提取目录");
                string newname = Path.Combine(directory, text + ".zip");
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
        if (sender is Button { DataContext: Manga book })
        {
            RepaetBooks.Remove(book);
        }
    }

    private void TagListOrder_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is not TagListOrder { DataContext: Manga manga } order)
        {
            return;
        }
        var items = BracketBasedStringParser.SplitByBrackets_KeepBracket(manga.FileDisplayName);
        order.Sources = items;
    }

    private void NewnameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox { DataContext: Manga book, Text: string text })
        {
            return;
        }
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
