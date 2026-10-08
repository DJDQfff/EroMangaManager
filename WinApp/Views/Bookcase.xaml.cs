using Core.Interfaces;
using UnoLibrary.Services;

namespace WinApp.Views.MainPageChildPages;

public sealed partial class Bookcase : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    readonly SettingViewModel settingViewModel;
    readonly CoverSetter CoverSetter;
    readonly MangaOperationViewModel MangaOperationViewModel;
    readonly ObservableCollectionVM ObservableCollectionVM;
    readonly SearchFilterViewModelFactory _filterFactory;

    /// <summary>TokenizingTextBox 的建议源（UI 自己维护）</summary>
    public ObservableCollection<string> AvailableTagsForUI { get; } = [];

    /// <summary>当前页面的筛选/分页 VM</summary>
    public SearchFilterViewModel? FilterVm
    {
        get;
        private set
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FilterVm)));
        }
    }

    /// <summary>当前选中的 group（仅用于 ComboBox 显示/切换）</summary>
    private MangasGroup? _currentGroup;

    public Bookcase(
        SettingViewModel _settingViewModel,
        MangaOperationViewModel _mangaOperationViewModel,
        ObservableCollectionVM _observableCollectionVM,
        CoverSetter _coversetter,
        SearchFilterViewModelFactory filterFactory
    )
    {
        InitializeComponent();
        CoverSetter = _coversetter;
        settingViewModel = _settingViewModel;
        MangaOperationViewModel = _mangaOperationViewModel;
        ObservableCollectionVM = _observableCollectionVM;
        ObservableCollectionVM.EventAfterDeleteMangaSource += manga =>
            FilterVm?.DisplayMangas.Remove(manga);
        _filterFactory = filterFactory;
    }

    #region 切换文件夹 / 分组

    private async void Combochangefolder_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e
    )
    {
        if (e.RemovedItems.Count > 0)
        {
            FilterVm = null;
            Bookcase_GridView.MangaSource = null!;
        }

        if (e.AddedItems.Count > 0 && e.AddedItems[0] is MangasGroup g)
        {
            await SwitchToGroup(g);
        }
    }

    /// <summary>切换到一个 group：建 VM、刷新、加载封面</summary>
    public async Task SwitchToGroup(MangasGroup group)
    {
        _currentGroup = group;

        FilterVm = _filterFactory.CreateForGroup(group);
        Bookcase_GridView.MangaSource = FilterVm.DisplayMangas;

        // 清空标签控件（新 VM 的 RequiredTags 是空的）
        //await TagTokenizingTextBox.ClearAsync();
        FilterVm.RequiredTags.Clear();
        // 同步 UI 到默认模式（分页）
        DisplayModeRadio.SelectedIndex = 1; // 事件里 FilterVm.IsPaged == true，return，无副作用
        // 初始：分页模式，第 1 页
        FilterVm.IsPaged = true;
        FilterVm.CurrentPage = 1;
        FilterVm.RefreshDisplay();

        UpdateHintVisibility();

        await CoverSetter.MultiLoadWork(FilterVm.DisplayMangas, true, true);
    }

    #endregion

    #region 显示模式 / 翻页

    private void DisplayModeRadio_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (FilterVm is null)
            return;

        bool paged = DisplayModeRadio.SelectedIndex == 1;

        // 值没变，跳过（避免首次弹出 Flyout 时重复刷新）
        if (FilterVm.IsPaged == paged)
            return;

        FilterVm.IsPaged = paged;
        FilterVm.CurrentPage = 1;
        FilterVm.RefreshDisplay();

        UpdateHintVisibility();
    }

    private async void Numberbox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (FilterVm is null)
            return;
        if (Math.Abs(args.NewValue - args.OldValue) < 0.001)
            return;

        FilterVm.CurrentPage = (int)args.NewValue; // 显式同步（TwoWay 应该已经做了，保险）
        FilterVm.RefreshDisplay();

        UpdateHintVisibility();

        await CoverSetter.MultiLoadWork(FilterVm.DisplayMangas, true, true);
    }
    #endregion

    #region 筛选按钮

    private async void ApplyFilter_Click(object sender, RoutedEventArgs e)
    {
        if (FilterVm is null)
            return;

        CollectFilterFromUI(); // 把 UI 上的条件写进 FilterVm
        FilterVm.ApplyFilterAndRefresh();

        numberbox.Value = 1;
        numberbox.Maximum = Math.Max(1, FilterVm.PageCount);

        UpdateHintVisibility();
        FilterFlyout.Hide();

        await CoverSetter.MultiLoadWork(FilterVm.DisplayMangas, true, true);
    }

    private async void ClearFilter_Click(object sender, RoutedEventArgs e)
    {
        if (FilterVm is null)
            return;

        // 清控件 → 触发 TokenItemChanged → 清 VM.RequiredTags
        FilterVm.RequiredTags.Clear();
        ResetFilterUI();
        FilterVm.ClearFilterAndRefresh();

        numberbox.Value = 1;
        numberbox.Maximum = Math.Max(1, FilterVm.PageCount);

        UpdateHintVisibility();

        await CoverSetter.MultiLoadWork(FilterVm.DisplayMangas, true, true);
    }

    #endregion

    #region 筛选 UI <-> VM

    /// <summary>把 XAML 上的控件状态写进 FilterVm</summary>
    private void CollectFilterFromUI()
    {
        if (FilterVm is null)
            return;

        FilterVm.NameKeyword = ChkName.IsChecked == true ? (TxtName.Text ?? "") : "";
        FilterVm.FilterByChapterName = ChkChapterName.IsChecked == true;
        FilterVm.ChapterNameKeyword =
            ChkChapterName.IsChecked == true ? (TxtChapterName.Text ?? "") : "";
        FilterVm.FilterByFileSize = ChkSize.IsChecked == true;
        FilterVm.MinFileSize = long.TryParse(TxtSizeMin.Text, out var mn) ? mn * 1024 * 1024 : 0;
        FilterVm.MaxFileSize = long.TryParse(TxtSizeMax.Text, out var mx) ? mx * 1024 * 1024 : 0;

        FilterVm.FilterByChapterAmount = ChkChapter.IsChecked == true;
        FilterVm.MinChapterAmount = int.TryParse(TxtChapterMin.Text, out var cmn)
            ? Math.Max(1, cmn)
            : 1;
        FilterVm.MaxChapterAmount = int.TryParse(TxtChapterMax.Text, out var cmx) ? cmx : 0;

        FilterVm.FileTypes.Clear();
        foreach (var cb in FilterTypePanel.Children.OfType<CheckBox>())
            if (cb.IsChecked == true && cb.Tag is string tag && !string.IsNullOrEmpty(tag))
                FilterVm.FileTypes.Add(tag);

        if (FilterVm.FileTypes.Count == 0) // 兜底：至少一个
        {
            FilterVm.FileTypes.Add("__folder__");
            FilterVm.FileTypes.Add(".zip");
        }
    }

    /// <summary>把 UI 控件复位到默认</summary>
    private void ResetFilterUI()
    {
        ChkName.IsChecked = false;
        TxtName.Text = "";
        ChkChapterName.IsChecked = false;
        TxtChapterName.Text = "";
        ChkSize.IsChecked = false;
        TxtSizeMin.Text = "";
        TxtSizeMax.Text = "";
        ChkChapter.IsChecked = false;
        TxtChapterMin.Text = "";
        TxtChapterMax.Text = "";
        ChkFolder.IsChecked = true;
        ChkZip.IsChecked = true;
        // 标签控件由调用方负责清空（ClearFilter_Click 里 await ClearAsync）
    }

    /// <summary>类型至少勾一个</summary>
    private void TypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (ChkFolder.IsChecked != true && ChkZip.IsChecked != true)
        {
            if (sender is CheckBox cb)
                cb.IsChecked = true;
        }
    }

    #endregion

    #region 标签筛选

    /// <summary>输入验证：只允许已存在的标签</summary>
    private void TagTokenBox_TokenItemAdding(
        TokenizingTextBox sender,
        TokenItemAddingEventArgs args
    )
    {
        if (FilterVm is null)
        {
            args.Cancel = true;
            return;
        }

        var t = args.TokenText;
        if (!FilterVm.AvailableTags.Contains(t, StringComparer.OrdinalIgnoreCase))
            args.Cancel = true;
    }

    /// <summary>用户手动增删 token 后，同步回 VM</summary>
    private void TagTokenBox_TokenItemChanged(TokenizingTextBox sender, object args)
    {
        //if (FilterVm is null)
        //    return;

        //FilterVm.RequiredTags.Clear();
        //foreach (var item in TagTokenizingTextBox.SelectedItems.OfType<string>())
        //    FilterVm.RequiredTags.Add(item);
    }

    /// <summary>输入时过滤建议</summary>
    private void TagTokenizingTextBox_TextChanged(
        AutoSuggestBox sender,
        AutoSuggestBoxTextChangedEventArgs args
    )
    {
        if (FilterVm is null)
            return;

        FilterVm.FiltTags(TagTokenizingTextBox.Text, AvailableTagsForUI);
    }

    #endregion

    #region 空状态

    private void UpdateHintVisibility()
    {
        bool empty = FilterVm is null || FilterVm.FilteredCount == 0;
        Bookcase_HintTextBlock.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;

        if (FilterVm is null)
            Bookcase_HintTextBlock.Text = /* NoSelectedFolder */
                "未选择文件夹";
        else
            Bookcase_HintTextBlock.Text = /* NoMatchResult */
                "无匹配结果";
    }

    #endregion

    #region 视图模板

    [RelayCommand]
    private async Task ChangeMangaGridViewDataTemplate(string index)
    {
        var key = "Template" + index;
        Bookcase_GridView.ItemTemplateIndex = key;
        settingViewModel.AppConfig.General.MangasGridViewDataTemplateKey = key;
    }

    #endregion
}
