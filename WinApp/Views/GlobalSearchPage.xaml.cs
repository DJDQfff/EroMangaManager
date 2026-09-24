// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板
namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class GlobalSearchPage : Page
{
    SearchMangaViewModel SearchMangaViewModel2 { get; }
    ObservableCollectionVM ObservableCollectionVM { get; }
    SettingViewModel SettingViewModel { get; }

    /// <summary>
    ///
    /// </summary>
    public GlobalSearchPage(
        SettingViewModel _settingViewModel,
        ObservableCollectionVM observableCollectionVM,
        CoverSetter coverSetter,
        SearchMangaViewModel _viewmodel,
        MangaOperationViewModel _mangaOperationViewModel
    )
    {
        InitializeComponent();
        ObservableCollectionVM = observableCollectionVM;
        SearchMangaViewModel2 = _viewmodel;
        ResultGridView.MangaOperationViewModel = _mangaOperationViewModel;
        SettingViewModel = _settingViewModel;

        ObservableCollectionVM.EventAfterDeleteMangaSource += x =>
            SearchMangaViewModel2.ResultMangas.Remove(x);

        SearchMangaViewModel2.ResultNewAddSingle += async x =>
            await coverSetter.SingleLoadWork(x, true, true);
        SearchMangaViewModel2.ResultNewAddMulti += async x =>
            await coverSetter.MultiLoadWork(x, true, true);
    }

    public async Task Search(object parameter)
    {
        switch (parameter)
        {
            case SearchParameter searchParameter:
                {
                    var tags = searchParameter.Tags;
                    foreach (var tag in tags)
                    {
                        MangaTagTokenizingTextBox.AddTokenItem(tag);
                    }
                    //SearchStartButton_Click(SearchStartButton , new RoutedEventArgs());
                }
                break;
            //直接把manga传进来，参数自己修改
            case Manga manga:
                {
                    SearchMangaViewModel2.RequiredText = manga.Name;

                    SearchMangaViewModel2.RequiredTags.Clear();
                    foreach (var tag in manga.Tags)
                    {
                        MangaTagTokenizingTextBox.AddTokenItem(tag);
                    }
                }
                break;

            case string manganame:
                {
                    SearchMangaViewModel2.RequiredText = manganame;
                    SearchMangaViewModel2.RequiredTags.Clear();
                }
                break;

            case IEnumerable<string> tags:
                {
                    SearchMangaViewModel2.RequiredText = string.Empty;
                    SearchMangaViewModel2.RequiredTags.Clear();
                    foreach (var tag in tags)
                    {
                        SearchMangaViewModel2.RequiredTags.Add(tag);
                    }
                }
                break;
        }
        await SearchMangaViewModel2.SearchAsync();
    }

    /// <summary>
    /// 输入验证/拦截：
    /// 当用户在标签搜索框中输入一个标签并尝试将其添加为 Token 时，
    /// 如果该标签在系统中不存在，
    /// 则取消添加（防止用户输入无效/拼写错误的标签）。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void TagTokenBox_TokenItemAdding(
        TokenizingTextBox sender,
        TokenItemAddingEventArgs args
    )
    {
        var t = args.TokenText;
        if (!SearchMangaViewModel2.AvailableTags.Contains(t))
        {
            args.Cancel = true;
        }
    }

    private async void TagTokenBox_TokenItemChanged(TokenizingTextBox sender, object args)
    {
        await SearchMangaViewModel2.SearchAsync();
    }

    private async void NameBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args
    )
    {
        await SearchMangaViewModel2.SearchAsync();
    }

    //private void ShowInBookcaseButton_Click (object sender , RoutedEventArgs e)
    //{
    //    var result = ResultGridView.ItemsSource;

    //    var condition = result as IEnumerable<Manga>;

    //    var mangasfolder = new MangasGroups()
    //    {
    //        ShowString = (
    //            NameAugoSuggestBox.Text + "+" + MangaTagTokenizingTextBox.SelectedTokenText
    //        ).Trim('+') ,
    //    };
    //    mangasfolder.Mangas.AddRange(condition);
    //    MainPage.Current.MainFrame.Navigate(typeof(Bookcase) , mangasfolder);
    //}

    private void TagTokenizingTextBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args
    )
    {
        SearchMangaViewModel2.FiltTags(MangaTagTokenizingTextBox.Text);
    }
}
