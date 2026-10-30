using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Xaml.Automation;
using Uno.Extensions.Hosting; // ✅ CreateBuilder 所在命名空间
using Uno.Extensions.Localization;
using UnoLibrary.Strings;

namespace UnoApp;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public static IServiceProvider Services { get; private set; } = null!;

    public App()
    {
        InitializeComponent();
    }

    //protected mainWindow? mainWindow { get; private set; }

    /// <summary>
    /// 根窗口。Windows / Skia 桌面端需要自己创建并持有引用，否则可能被回收后窗口关闭
    /// </summary>
    public static Window? RootWindow { get; private set; }

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            var builder = this.CreateBuilder(args)
                .Configure(host =>
                {
                    host.UseLocalization()
                        .ConfigureServices(
                            (context, services) =>
                            {
                                services.AddSingleton<IPages, Pages>();
                                services.AddSingleton<MainPage>();
                                services.AddTransient<SettingPage>();
                                services.AddSingleton<ServerStorage>();
                                services.AddSingleton<MangaAPIClient>();
                                services.AddSingleton<RemoteMangaViewModel>();
                                services.AddSingleton<NavigationPage>();
                            }
                        );
                });
            Services = builder.Build().Services;

            // 共享主界面（UnoLibrary.MainWindow）；移动端的安全区由 MainWindow 自己处理
            var shell = new MainWindow();
            shell.IsTitleBarVisible = false;
            FrameworkElement root = shell;

#if UNO_SINGLE_WINDOW
            // 单窗口平台：Uno 启动时已创建唯一的窗口，只能复用它，不能 new Window()
            var window = Window.Current ?? new Window();
#else
            // Windows / Skia 桌面端支持多窗口，自己创建
            var window = new Window();
#endif
            RootWindow = window;
            window.Content = root;

            // 与 WinApp 一致：在 App 里解析根页面并把 IServiceProvider 交给它
            var mainPage = Services.GetRequiredService<MainPage>();
            mainPage.ServiceProvider = Services;
            shell.SetPage<MainPage>(mainPage);

            window.SetWindowIcon();
            window.Activate();

            // UnoApp 比 WinApp 多一步：Android 上先确认能连上服务端（连不上就弹强制连接框）
            if (OperatingSystem.IsAndroid())
            {
                await EnsureServerConnectedAsync(root);
            }

            // 连接成功后再让外壳导航到 NavigationPage
            if (mainPage.NavigateToPage(StringsEnum.Bookcase) is NavigationPage navigationPage)
            {
                await navigationPage.OnNavigatedTo();
            }
        }
        catch (Exception ex)
        {
            // OnLaunched 是 async void，异常原本会被 Uno 调度器吞掉，界面只会一直停在闪屏
            System.Diagnostics.Debug.WriteLine(ex);
            ShowStartupError(ex);
        }
    }

    /// <summary>
    /// Android 启动时先确认能连上服务端；连不上就弹出强制连接对话框（原来在 MainWindow 的
    /// OnRootContentLoaded 里靠 Loaded 事件触发，现在改为 App 里显式调用）。
    /// </summary>
    private static async Task EnsureServerConnectedAsync(FrameworkElement root)
    {
        var client = Services.GetRequiredService<MangaAPIClient>();
        var storage = Services.GetRequiredService<ServerStorage>();

        try
        {
            if (await client.CheckConnectionAsync())
            {
                return;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }

        // 等可视树挂载后再弹框，否则 XamlRoot 为空会抛异常
        await WaitForLoadedAsync(root);

        ConnectDialog connectDialog = new(storage, client) { XamlRoot = root.XamlRoot };
        await connectDialog.ShowAsync();
    }

    /// <summary> 等待元素进入可视树（已经 Loaded 则立即返回） </summary>
    private static Task WaitForLoadedAsync(FrameworkElement element)
    {
        if (element.IsLoaded)
        {
            return Task.CompletedTask;
        }

        var tcs = new TaskCompletionSource();

        void OnLoaded(object? sender, RoutedEventArgs e)
        {
            element.Loaded -= OnLoaded;
            tcs.TrySetResult();
        }

        element.Loaded += OnLoaded;
        return tcs.Task;
    }

    private static void ShowStartupError(Exception ex)
    {
        var text = new TextBlock
        {
            Text = ex.ToString(),
            TextWrapping = TextWrapping.Wrap,
            Margin = new Thickness(16),
        };
#if UNO_SINGLE_WINDOW
        if (Window.Current is { } window)
        {
            window.Content = text;
        }
#else
        new Window { Content = text }.Activate();
#endif
    }

    /// <summary>
    /// Configures global Uno Platform logging
    /// </summary>
    public static void InitializeLogging()
    {
#if DEBUG
        // Logging is disabled by default for release builds, as it incurs a significant
        // initialization cost from Microsoft.Extensions.Logging setup. If startup performance
        // is a concern for your application, keep this disabled. If you're running on the web or
        // desktop targets, you can use URL or command line parameters to enable it.
        //
        // For more performance documentation: https://platform.uno/docs/articles/Uno-UI-Performance.html

        var factory = LoggerFactory.Create(builder =>
        {
#if __WASM__
            builder.AddProvider(
                new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider()
            );
#elif __IOS__
            builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());

            // Log to the Visual Studio Debug console
            builder.AddConsole();
#else
            builder.AddConsole();
#endif

            // Exclude logs below this level
            builder.SetMinimumLevel(LogLevel.Information);

            // Default filters for Uno Platform namespaces
            builder.AddFilter("Uno", LogLevel.Warning);
            builder.AddFilter("Windows", LogLevel.Warning);
            builder.AddFilter("Microsoft", LogLevel.Warning);

            // Generic Xaml events
            // builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace );

            // Layouter specific messages
            // builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug );

            // builder.AddFilter("Windows.Storage", LogLevel.Debug );

            // Binding related messages
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );
            // builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug );

            // Binder memory references tracking
            // builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug );

            // DevServer and HotReload related
            // builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

            // Debug JS interop
            // builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug );
        });

        global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
        global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
#endif
    }
}
