using UnoLibrary.Strings;

namespace UnoApp;

public sealed partial class MainWindow : Window
{
    readonly ServerStorage _serverStorage;
    readonly MangaAPIClient _mangaApiClient;
    readonly MainPage _mainPage;

    public MainWindow(ServerStorage serverStorage, MainPage mainPage, MangaAPIClient mangaApiClient)
    {
        InitializeComponent();
        _serverStorage = serverStorage;
        _mangaApiClient = mangaApiClient;
        mainPage.ServiceProvider = App.Services;
        mainFrame.Content = mainPage;
        _mainPage = mainPage;

        mainPage.Loaded += OnRootContentLoaded;
    }

    /// <summary>
    /// 由 App.cs 调用，用于启动初始化流程
    /// </summary>
    public void StartInitialization()
    {
        // 确保 Content 已经准备好，监听其 Loaded 事件
        if (this.Content is FrameworkElement rootElement)
        {
            rootElement.Loaded += OnRootContentLoaded;
        }
    }

    private async void OnRootContentLoaded(object sender, RoutedEventArgs e)
    {
        // 1. 取消订阅，防止 Loaded 事件被重复触发
        if (sender is FrameworkElement element)
        {
            element.Loaded -= OnRootContentLoaded;
        }
        // 2. 先尝试自动连接
        try
        {
            _ = await _mangaApiClient.CheckConnectionAsync();
        }
        catch (Exception)
        {
            // 2. 此时 XamlRoot 绝对有效，可以安全弹出强制连接对话框
            ConnectDialog connectDialog = new(_serverStorage, _mangaApiClient)
            {
                XamlRoot = this.Content!.XamlRoot,
            };

            await connectDialog.ShowAsync();
        }
        if (_mainPage.NavigateToPage(StringsEnum.Bookcase) is NavigationPage page)
        {
            await page.OnNavigatedTo();
        }

        // 3. 对话框成功关闭（说明连接成功），加载主界面
        //if (this.Content is SafeArea { Content: ContentControl rootFrame })
        //{
        //    rootFrame.Content = App.Services.GetRequiredService<SettingPage>();
        //}
    }
}
