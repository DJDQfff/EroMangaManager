// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace WinApp.Views.MainPageChildPages;

/// <summary>
/// 可用于自身或导航至 Frame 内部的空白页。
/// </summary>
public sealed partial class Bookcase : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    readonly SettingViewModel settingViewModel;
    readonly ObservableCollectionVM ViewModel;
    readonly CoverSetter coverSetter;
    public MangasGroup? MangasGroup
    {
        get;
        set
        {
            field = value;
            if (value is null)
            {
                Bookcase_GridView.ItemsSource = null!;
                Bookcase_HintTextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                Bookcase_GridView.ItemsSource = value.DisplayMangas;
                Bookcase_HintTextBlock.Visibility = Visibility.Collapsed;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MangasGroup)));
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    public Bookcase(
        CoverSetter _coverSetter,
        SettingViewModel _settingViewModel,
        ObservableCollectionVM _ViewModel
    )
    {
        InitializeComponent();
        coverSetter = _coverSetter;
        settingViewModel = _settingViewModel;
        ViewModel = _ViewModel;
    }

    /// <summary>
    /// 导航时，传入要绑定的数据
    /// </summary>
    /// <param name="e"></param>
    public async Task OnNavigated(MangasGroup e)
    {
        MangasGroup = e;
        await coverSetter.MultiLoadWork(MangasGroup.DisplayMangas, true, false);
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
            MangasGroup = e.AddedItems[0] as MangasGroup;
            await coverSetter.MultiLoadWork(MangasGroup!.DisplayMangas, true, true);
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

            MangasGroup.Display((newvalue - 1) * 20, 20);
            await App
                .Services.GetRequiredService<CoverSetter>()
                .MultiLoadWork(MangasGroup.DisplayMangas, true, true);
        }
    }
}
