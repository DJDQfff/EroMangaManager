// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板
namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class GlobalSearchPage : Page
{
    readonly ObservableCollectionVM viewmodel;
    readonly SettingViewModel settingViewModel;

    /// <summary>
    ///
    /// </summary>
    public GlobalSearchPage(
        ObservableCollectionVM _viewmodel,
        SettingViewModel _settingViewModel,
        CoverSetter coverSetter
    )
    {
        InitializeComponent();
        viewmodel = _viewmodel;
        settingViewModel = _settingViewModel;

        viewmodel.EventAfterDeleteMangaSource += x =>
        {
            viewmodel.RemoveManga(x); /*viewmodel.SearchResultMangas.Remove(x);*/
        };

        viewmodel.SearchResultNewAddSingle += async x =>
            await coverSetter.SingleLoadWork(x, true, true);
        viewmodel.SearchResultNewAddMulti += async x =>
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
                    viewmodel.SearchRequiredText = manga.Name;

                    viewmodel.SearchRequiredTags.Clear();
                    foreach (var tag in manga.Tags)
                    {
                        MangaTagTokenizingTextBox.AddTokenItem(tag);
                    }
                }
                break;

            case string manganame:
                {
                    viewmodel.SearchRequiredText = manganame;
                    viewmodel.SearchRequiredTags.Clear();
                }
                break;

            case IEnumerable<string> tags:
                {
                    viewmodel.SearchRequiredText = string.Empty;
                    viewmodel.SearchRequiredTags.Clear();
                    foreach (var tag in tags)
                    {
                        viewmodel.SearchRequiredTags.Add(tag);
                    }
                }
                break;
        }
        await viewmodel.Search();
    }

    // TODO 搞不清这个干嘛的
    private void TagTokenBox_TokenItemAdding(
        TokenizingTextBox sender,
        TokenItemAddingEventArgs args
    )
    {
        var t = args.TokenText;
        if (!viewmodel.AllTags.Contains(t))
        {
            args.Cancel = true;
        }
    }

    private async void TagTokenBox_TokenItemChanged(TokenizingTextBox sender, object args)
    {
        await viewmodel.Search();
    }

    private async void NameBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args
    )
    {
        await viewmodel.Search();
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
        viewmodel.FiltSearchTags(MangaTagTokenizingTextBox.Text);
    }
}
