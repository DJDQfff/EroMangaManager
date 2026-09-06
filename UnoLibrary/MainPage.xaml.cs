// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace UnoLibrary;

/// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
public sealed partial class MainPage : Page
{
    public IServiceProvider ServiceProvider { get; set; } = null!;
    IPages Pages { get; set; } = null!;

    /// <summary>
    ///
    /// </summary>
    public MainPage(IPages pages)
    {
        InitializeComponent();
        Pages = pages;
        MainNavigationView.MenuItemsSource = pages.MainPages;

        MainNavigationView.FooterMenuItemsSource = pages.FooterPages;
    }

    public void OnNavigated()
    {
        //NavigateToPage<Bookcase>();
        NavigateToPage(StringsEnum.Bookcase);
    }

    private void MainNavigationView_ItemInvoked(
        NavigationView sender,
        NavigationViewItemInvokedEventArgs args
    )
    {
        if (args.IsSettingsInvoked)
        {
            //NavigateToPage<SettingPage>();
            NavigateToPage(StringsEnum.Setting);
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

    public object NavigateToPage(StringsEnum uid)
    {
        var item = Pages.Find(uid);
        var pageType = item.Page;
        var page = ServiceProvider.GetRequiredService(pageType);
        MainNavigationView.SelectedItem = item;
        PageContainer.Content = null;
        PageContainer.Content = page;
        return page;
    }

    public TPage NavigateToPage<TPage>()
        where TPage : Page
    {
        var page = ServiceProvider.GetRequiredService<TPage>();

        // 选中菜单项的逻辑与 page 实例无关，只依赖类型
        if (MainNavigationView.MenuItemsSource is IEnumerable<NavigationItem> items)
        {
            MainNavigationView.SelectedItem = items.Single(x => x.Page == typeof(TPage));
        }
        PageContainer.Content = null;
        PageContainer.Content = page;

        return page;
    }
}
