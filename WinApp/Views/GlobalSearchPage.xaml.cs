namespace WinApp.Views.MainPageChildPages;

public sealed partial class GlobalSearchPage : Page
{
    SearchFilterViewModel SearchVm { get; }
    ObservableCollectionVM ObservableCollectionVM { get; }
    SettingViewModel SettingViewModel { get; }

    /// <summary>TokenizingTextBox 的建议源（UI 自己维护）</summary>
    public ObservableCollection<string> AvailableTagsForUI { get; } = [];

    public GlobalSearchPage(
        SettingViewModel _settingViewModel,
        ObservableCollectionVM observableCollectionVM,
        CoverSetter coverSetter,
        SearchFilterViewModelFactory filterFactory,
        MangaOperationViewModel _mangaOperationViewModel
    )
    {
        SearchVm = filterFactory.CreateForGlobal();

        InitializeComponent();

        ObservableCollectionVM = observableCollectionVM;
        SettingViewModel = _settingViewModel;

        ResultGridView.MangaOperationViewModel = _mangaOperationViewModel;
        ResultGridView.MangaSource = SearchVm.DisplayMangas;

        // 删除本子时同步结果
        ObservableCollectionVM.EventAfterDeleteMangaSource += x => SearchVm.DisplayMangas.Remove(x);
    }

    #region 外部调用入口

    /// <summary>
    /// 程序化设置搜索条件：只改 VM，控件靠 ItemsSource 绑定显示。
    /// </summary>
    public async Task Search(object parameter)
    {
        switch (parameter)
        {
            case SearchParameter sp:
                SearchVm.NameKeyword = "";
                SearchVm.RequiredTags.Clear();
                foreach (var tag in sp.Tags)
                    SearchVm.RequiredTags.Add(tag);
                break;

            case Manga manga:
                SearchVm.NameKeyword = manga.Name;
                SearchVm.RequiredTags.Clear();
                foreach (var tag in manga.Tags)
                    SearchVm.RequiredTags.Add(tag);
                break;

            case string manganame:
                SearchVm.NameKeyword = manganame;
                SearchVm.RequiredTags.Clear();
                break;

            case IEnumerable<string> tags:
                SearchVm.NameKeyword = "";
                SearchVm.RequiredTags.Clear();
                foreach (var tag in tags)
                    SearchVm.RequiredTags.Add(tag);
                break;
        }

        await SearchVm.SearchAsync();
    }

    #endregion

    #region 名称

    private async void NameBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args
    )
    {
        // 只在用户输入时触发，避免程序设置 Text 时重复搜索
        if (args.Reason != AutoSuggestionBoxTextChangeReason.UserInput)
            return;

        // TwoWay 绑定已经把 Text 写进 SearchVm.NameKeyword，这里只负责搜索
        await SearchVm.SearchAsync();
    }

    #endregion

    #region 标签

    /// <summary>输入验证：只允许已存在的标签</summary>
    private void TagTokenBox_TokenItemAdding(
        TokenizingTextBox sender,
        TokenItemAddingEventArgs args
    )
    {
        var t = args.TokenText;
        if (!SearchVm.AvailableTags.Contains(t, StringComparer.OrdinalIgnoreCase))
            args.Cancel = true;
    }

    /// <summary>用户手动增删 token 后，同步回 VM 并重搜</summary>
    private async void TagTokenBox_TokenItemChanged(TokenizingTextBox sender, object args)
    {
        //SearchVm.RequiredTags.Clear();
        //foreach (var item in MangaTagTokenizingTextBox.SelectedItems.OfType<string>())
        //    SearchVm.RequiredTags.Add(item);

        await SearchVm.SearchAsync();
    }

    /// <summary>输入时过滤建议</summary>
    private void TagTokenizingTextBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args
    )
    {
        SearchVm.FiltTags(MangaTagTokenizingTextBox.Text, AvailableTagsForUI);
    }

    #endregion
}
