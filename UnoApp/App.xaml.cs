using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.UI.Xaml.Automation;
using Uno.Extensions.Hosting; // ✅ CreateBuilder 所在命名空间
using Uno.Extensions.Localization;

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
                                services.AddSingleton<MainWindow>(); // UserControl，不是 Window
                                services.AddSingleton<RemoteMangaViewModel>();
                                services.AddSingleton<NavigationPage>();
                            }
                        );
                });
            Services = builder.Build().Services;

            // MainWindow 只是 UserControl，所以这一行在 Android/iOS/WASM 上也能正常返回
            var shell = Services.GetRequiredService<MainWindow>();

#if UNO_SINGLE_WINDOW
            // 单窗口平台：Uno 启动时已创建唯一的窗口，只能复用它，不能 new Window()
            var window = Window.Current ?? new Window();
#else
            // Windows / Skia 桌面端支持多窗口，自己创建
            var window = new Window();
#endif
            RootWindow = window;
            window.Content = shell;
            window.SetWindowIcon();
            window.Activate();
        }
        catch (Exception ex)
        {
            // OnLaunched 是 async void，异常原本会被 Uno 调度器吞掉，界面只会一直停在闪屏
            System.Diagnostics.Debug.WriteLine(ex);
            ShowStartupError(ex);
        }
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
