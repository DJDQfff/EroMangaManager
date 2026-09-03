// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

using WinApp.Strings;

namespace WinApp.Views;


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
            new(typeof(Bookcase),StringsExtension.ResourceLoader.GetString(StringsEnum.Bookcase.ToString()) , new(Symbol.ViewAll)),
            new(typeof(LibraryPage), StringsExtension.ResourceLoader.GetString(StringsEnum.Library.ToString()), new(Symbol.Library)),
            new(typeof(GlobalSearchPage), StringsExtension.ResourceLoader.GetString(StringsEnum.GlobalSearch.ToString()), new(Symbol.Find)),
            new(typeof(TagsManagePage), StringsExtension.ResourceLoader.GetString(StringsEnum.MangaTagsManage.ToString()), new(Symbol.Manage)),
            new(typeof(FindSameManga), StringsExtension.ResourceLoader.GetString(StringsEnum.FindSameMangaByName.ToString()), new(Symbol.Copy)),
            //new(typeof(RemoveRepeatTags2), StringsExtension.ResourceLoader.GetString(StringsEnum.RemoveRepeatTags.ToString()), new(Symbol.Tag)),
            new(typeof(IrregularNameSearch), StringsExtension.ResourceLoader.GetString(StringsEnum.IrregularName.ToString()), new(Symbol.Edit)),
            new(typeof(ServerPage), StringsExtension.ResourceLoader.GetString(StringsEnum.Server.ToString()), new(Symbol.Remote)),
        };

        MainNavigationView.FooterMenuItemsSource = new NavigationItem[]
        {
            new(typeof(UsageDocumentPage), StringsExtension.ResourceLoader.GetString(StringsEnum.Usage.ToString()), new(Symbol.Help)),
            //new(typeof(UpdateRecordsPage), StringsExtension.ResourceLoader.GetString(StringsEnum.UpdateRecords.ToString()), new(Symbol.ShowResults) ),
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
