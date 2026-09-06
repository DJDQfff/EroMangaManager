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

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
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
                            services.AddSingleton<MainWindow>();
                            services.AddSingleton<RemoteMangaViewModel>();
                            services.AddSingleton<NavigationPage>();
                        }
                    );
            });
        Services = builder.Build().Services;

        var mainWindow = Services.GetRequiredService<MainWindow>();
#if DEBUG
        //mainWindow.UseStudio();
#endif
        // Do not repeat app initialization when the Window already has content,
        // just ensure that the window is active
        if (mainWindow.Content is not SafeArea { Content: ContentControl rootFrame })
        {
            // Create a Frame to act as the navigation context and navigate to the first page
            rootFrame = new();

            // Place the frame in the current Window
            mainWindow.Content = rootFrame;
        }

        mainWindow.SetWindowIcon();
        // Ensure the current window is active
        mainWindow.Activate();
        //mainWindow.StartInitialization();
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
