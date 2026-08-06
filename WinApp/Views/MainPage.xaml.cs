// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace WinApp.Views;

internal record NavigationItem(Type Page, StringsEnum Uid, SymbolIcon Icon)
{
    public string UidValue => StringsExtension.ResourceLoader.GetString(Uid.ToString());
}

/// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
public sealed partial class MainPage : Page
{
    public IServiceProvider ServiceProvider { get; set; } = null!;

    /// <summary>
    ///
    /// </summary>
    public MainPage()
    {
        InitializeComponent();

        MainNavigationView.MenuItemsSource = new NavigationItem[]
        {
            new(typeof(Bookcase), StringsEnum.Bookcase, new(Symbol.ViewAll)),
            new(typeof(LibraryPage), StringsEnum.Library, new(Symbol.Library)),
            new(typeof(GlobalSearchPage), StringsEnum.GlobalSearch, new(Symbol.Find)),
            new(typeof(TagsManagePage), StringsEnum.MangaTagsManage, new(Symbol.Manage)),
            new(typeof(FindSameManga), StringsEnum.FindSameMangaByName, new(Symbol.Copy)),
            //new(typeof(RemoveRepeatTags2), StringsEnum.RemoveRepeatTags, new(Symbol.Tag)),
            new(typeof(IrregularNameSearch), StringsEnum.IrregularName, new(Symbol.Edit)),
            new(typeof(ServerPage), StringsEnum.Server, new(Symbol.Remote)),
        };

        MainNavigationView.FooterMenuItemsSource = new NavigationItem[]
        {
            new(typeof(UsageDocumentPage), StringsEnum.Usage, new(Symbol.Help)),
            //new(typeof(UpdateRecordsPage), StringsEnum.UpdateRecords, new(Symbol.ShowResults) ),
        };
    }

    public void OnNavigated()
    {
        NavigateToPage<Bookcase>();
    }

    private void MainNavigationView_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args
    )
    {
        if (args.IsSettingsInvoked)
        {
            NavigateToPage<SettingPage>();
            return;
        }
        if (
            args.InvokedItemContainer is NavigationViewItem
            {
                DataContext: NavigationItem navigationItem
            }
        )
        {
            var page = ServiceProvider.GetRequiredService(navigationItem.Page);
            PageContainer.Content = null;
            PageContainer.Content = page;
        }
    }

    public TPage NavigateToPage<TPage>()
        where TPage : Page
    {
        var page = ServiceProvider.GetRequiredService<TPage>();

        // 选中菜单项的逻辑与 page 实例无关，只依赖类型
        if (MainNavigationView.MenuItemsSource is IEnumerable<NavigationItem> items)
        {
            MainNavigationView.SelectedItem = items.SingleOrDefault(x => x.Page == typeof(TPage));
        }
        PageContainer.Content = null;
        PageContainer.Content = page;

        return page;
    }
}
