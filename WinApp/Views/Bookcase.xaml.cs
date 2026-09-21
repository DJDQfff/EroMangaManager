// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using Core.Interfaces;
using UnoLibrary.Services;

namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class Bookcase : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private readonly MangaFilterOptions _filter = new();
    readonly SettingViewModel settingViewModel;
    readonly CoverSetter CoverSetter;
    readonly MangaOperationViewModel MangaOperationViewModel;
    readonly ObservableCollectionVM ObservableCollectionVM;
    public MangasGroup? MangasGroup
    {
        get;
        set
        {
            field = value;
            if (value is null)
            {
                Bookcase_GridView.MangaSource = null!;
                Bookcase_HintTextBlock.Visibility = Visibility.Visible;
                Bookcase_HintTextBlock.Text =
                    "" /* NoSelectedFolder */
                ;
            }
            else
            {
                value.RefreshDisplay(paged: true, page: 1);
                Bookcase_GridView.MangaSource = value.DisplayMangas;
                Bookcase_HintTextBlock.Visibility = Visibility.Collapsed;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MangasGroup)));
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public Bookcase(
        SettingViewModel _settingViewModel,
        MangaOperationViewModel _mangaOperationViewModel,
        ObservableCollectionVM _observableCollectionVM,
        CoverSetter _coversetter
    )
    {
        InitializeComponent();
        CoverSetter = _coversetter;
        settingViewModel = _settingViewModel;
        MangaOperationViewModel = _mangaOperationViewModel;
        ObservableCollectionVM = _observableCollectionVM;
    }

    /// <summary>
    /// 导航时，传入要绑定的数据
    /// </summary>
    /// <param name="e"></param>
    public async Task OnNavigated(MangasGroup e)
    {
        MangasGroup = e;
        await CoverSetter.MultiLoadWork(MangasGroup.DisplayMangas, true, false);
    }

    //private async void ApplyFilter_Click (object sender , RoutedEventArgs e)
    //{

    //    // 获取筛选参数 还没有作这个控件
    //    //string? name = string.IsNullOrWhiteSpace(FilterName.Text) ? null : FilterName.Text;

    //    int chapterAmount = ChapterTypeRadio.SelectedIndex;

    //    long fileSize = 0;
    //    if (long.TryParse(FilterSizeMin.Text , out long sizeMin))
    //        fileSize = sizeMin * 1024 * 1024;

    //    // 调用 group.Filter 方法

    //    //MangasGroups?.Filter(null , chapterAmount , fileSize);

    //    // 刷新显示
    //    numberbox.Maximum = (MangasGroups?.Mangas.Count + 19) / 20 ?? 1;
    //    //numberbox.Value = 1;
    //    MangasGroups. Display (0 , 20);
    //    await App.Current.CoverSetter.AppendLoadWorks(MangasGroups.DisplayMangas , true , false);

    //}

    //private void ClearFilter_Click (object sender , RoutedEventArgs e)
    //{
    //    FilterTypePanel.Children.OfType<CheckBox>().ToList().ForEach(cb => cb.IsChecked = false);
    //    ChapterTypeRadio.SelectedIndex = 0;
    //    FilterSizeMin.Text = "";
    //    FilterSizeMax.Text = "";

    //    numberbox.Value = 1;
    //    MangasGroups.Filter(null , 0 , 0);
    //     MangasGroups.  Display(0 , 20);
    //}
    //TODO 本子名翻译功能。 因为原来的Bookcase被拆分为Bookcase和Bookcase两个类，所以这个方法现在有bug
    //private async void TranslateEachName (object sender , RoutedEventArgs e)
    //{
    //    var button = sender as AppBarButton;
    //    button.IsEnabled = false;

    //    try
    //    {
    //        await Translator.TranslateAllName();
    //    }
    //    catch { }

    //    var items = Bookcase_GridView.Items;
    //    foreach (var item in items)
    //    {
    //        var manga = item as Manga;
    //        var grid = Bookcase_GridView.ContainerFromItem(item) as GridViewItem;
    //        var root = grid.ContentTemplateRoot as Grid;
    //        var run = root.FindName("runtext") as Microsoft.UI.Xaml.Documents.Run;
    //        run.Text = manga.TranslatedName;
    //    }

    //    button.IsEnabled = true;
    //}

    private void Order(object sender, RoutedEventArgs e)
    {
        MangasGroup?.SortMangas(x => x.FileSize);
    }

    private async void Combochangefolder_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e
    )
    {
        // TODO 这个change事件，操作已缓存page实例正常。
        // 但如果页面每次都重新创建，就会出现问题。
        // 如调用NavigateToPage<Bookcase>().OnNavigated(MangasGroup)后，
        // ComboBox的SelectionChanged事件会再次触发
        if (e.RemovedItems.Count > 0)
        {
            MangasGroup = null;
        }
        if (e.AddedItems.Count > 0)
        {
            // 切文件夹：清筛选、回全部模式第 1 页
            ResetFilterUI();
            _filter.ClearAll();

            var g = e.AddedItems[0] as MangasGroup;
            if (g is not null)
            {
                g.ClearFilter();

                // 先设字段，再刷新（setter 里会 RefreshDisplay 并赋 MangaSource）
                MangasGroup = g;

                DisplayModeRadio.SelectedIndex = 1;
                numberbox.Value = 1;

                await CoverSetter.MultiLoadWork(g.DisplayMangas, true, true);
            }
        }
    }

    [RelayCommand]
    private async Task ChangeMangaGridViewDataTemplate(string index)
    {
        var key = "Template" + index;
        Bookcase_GridView.ItemTemplateIndex = key;
        settingViewModel.AppConfig.General.MangasGridViewDataTemplateKey = key;
    }

    private async void Numberbox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (MangasGroup != null)
        {
            var newvalue = (int)args.NewValue;

            RefreshDisplay();
            await CoverSetter.MultiLoadWork(MangasGroup.DisplayMangas, true, true);
        }
    }

    private void DisplayModeRadio_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        bool paged = DisplayModeRadio.SelectedIndex == 1;
        numberbox.IsEnabled = paged;
        if (!paged)
            numberbox.Value = 1;
        RefreshDisplay();
    }

    private async void ApplyFilter_Click(object sender, RoutedEventArgs e)
    {
        if (MangasGroup is null)
            return;

        CollectFilterFromUI();
        MangasGroup.ApplyFilter(_filter);

        numberbox.Value = 1;
        RefreshDisplay();

        FilterFlyout.Hide();
        await CoverSetter.MultiLoadWork(MangasGroup.DisplayMangas, true, true);
    }

    private async void ClearFilter_Click(object sender, RoutedEventArgs e)
    {
        if (MangasGroup is null)
            return;

        ResetFilterUI();
        _filter.ClearAll();
        MangasGroup.ClearFilter();

        numberbox.Value = 1;
        RefreshDisplay();

        await CoverSetter.MultiLoadWork(MangasGroup.DisplayMangas, true, true);
    }

    /// <summary>唯一刷新入口：重算 DisplayMangas 并重新赋 MangaSource</summary>
    private void RefreshDisplay()
    {
        if (MangasGroup is null)
            return;

        bool paged = DisplayModeRadio.SelectedIndex == 1;

        // 更新 NumberBox 范围
        int pageCount = MangasGroup.PageCount;
        numberbox.Maximum = Math.Max(1, pageCount);
        if (numberbox.Value > numberbox.Maximum)
            numberbox.Value = numberbox.Maximum;

        int page = (int)numberbox.Value;
        MangasGroup.RefreshDisplay(paged, page);

        // 关键：换引用，让 x:Bind 刷新
        Bookcase_GridView.MangaSource = MangasGroup.DisplayMangas.ToList();

        // 空状态
        if (MangasGroup.CurrentCount == 0)
        {
            Bookcase_HintTextBlock.Visibility = Visibility.Visible;
            Bookcase_HintTextBlock.Text =
                "" /* TODO NoMatchResult */
            ;
        }
        else
        {
            Bookcase_HintTextBlock.Visibility = Visibility.Collapsed;
        }
    }

    private void CollectFilterFromUI()
    {
        _filter.FilterByName = ChkName.IsChecked == true;
        _filter.NameKeyword = TxtName.Text ?? "";

        _filter.FilterByFileSize = ChkSize.IsChecked == true;
        _filter.MinFileSize = long.TryParse(TxtSizeMin.Text, out var mn) ? mn * 1024 * 1024 : 0;
        _filter.MaxFileSize = long.TryParse(TxtSizeMax.Text, out var mx) ? mx * 1024 * 1024 : 0;

        _filter.FileTypes.Clear();
        bool any = false;
        foreach (var cb in FilterTypePanel.Children.OfType<CheckBox>())
        {
            if (cb.IsChecked == true && cb.Tag is string tag && !string.IsNullOrEmpty(tag))
            {
                _filter.FileTypes.Add(tag);
                any = true;
            }
        }
        if (!any)
        {
            _filter.FileTypes.Add("__folder__");
            _filter.FileTypes.Add(".zip");
        }

        _filter.FilterByTag = ChkTag.IsChecked == true;
        _filter.TagKeyword = TxtTag.Text ?? "";
    }

    private void ResetFilterUI()
    {
        ChkName.IsChecked = false;
        TxtName.Text = "";

        ChkSize.IsChecked = false;
        TxtSizeMin.Text = "";
        TxtSizeMax.Text = "";

        ChkFolder.IsChecked = true;
        ChkZip.IsChecked = true;

        ChkTag.IsChecked = false;
        TxtTag.Text = "";
    }

    private void TypeCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if (ChkFolder.IsChecked != true && ChkZip.IsChecked != true)
        {
            // 回退：不允许一个都不勾
            if (sender is CheckBox cb)
                cb.IsChecked = true;
        }
    }
}
