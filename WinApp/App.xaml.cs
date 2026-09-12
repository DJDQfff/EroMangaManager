// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;

namespace WinApp;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();

        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPages, Pages>();
        //无其他依赖项
        services.AddTransient<ClipboardHelper>();
        services.AddTransient<ZipEntryHelper>();
        services.AddTransient<MangaStreamProvider>();
        services.AddTransient<Exporter>();
        services.AddTransient<MangaIO, MangaIO_WinAppSDK>();
        services.AddSingleton<CoverSetter>();
        services.AddSingleton<ObservableCollectionVM>();
        services.AddTransient<ISettingFilePath, WinUISetting>();
        services.AddTransient<StorageFolderHelper>();
        //依赖前面的
        services.AddTransient<Translator>();
        services.AddSingleton<StorageOperation>();
        services.AddSingleton<CoverHelper>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<Window>(sp => sp.GetRequiredService<MainWindow>());
        // Window 和 MainWindow 注入的是同一个实例
        services.AddSingleton<MangaFactory>();
        services.AddSingleton<SettingViewModel>();
        services.AddTransient<TagCategorySelect>();
        services.AddSingleton<ManageTagsViewModel2>();
        services.AddTransient<ContentDialogCreater>();
        //Pages
        services.AddSingleton<MainPage>();
        services.AddTransient<CommonSettingPage>();
        services.AddTransient<SettingPage>();
        services.AddSingleton<Bookcase>();
        services.AddTransient<UsageDocumentPage>();
        services.AddTransient<UpdateRecordsPage>();
        services.AddTransient<LibraryPage>();
        services.AddTransient<GlobalSearchPage>();
        services.AddTransient<TagsManagePage>();
        services.AddSingleton<FindSameManga>();
        services.AddTransient<IrregularNameSearch>();
        services.AddSingleton<ServerPage>();
        services.AddSingleton<INotifier, Notifier>();
        //数据库
        services.AddDbContextFactory<DataBase_Version3>(options =>
            options.UseSqlite(
                $"Data Source={ApplicationData.Current.LocalFolder.Path}\\localdatabase.db"
            )
        );
        services.AddTransient<DatabaseController>();
        Services = services.BuildServiceProvider();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        Services.GetRequiredService<CoverSetter>().SetCover += async manga =>
        {
            if (manga.CoverUri.EndsWith(".svg"))
            {
                manga.CoverUri = await Services.GetRequiredService<MangaIO>().GetCoverFile(manga);
            }
        };
        Services.GetRequiredService<CoverSetter>().MangaInfo += async manga =>
        {
            if (manga.FileSize == 0)
            {
                await Services.GetRequiredService<MangaIO>().LoadMangaInfo(manga);
            }
        };

        //DatabaseConfig.ConnectingString = $"Data Source={ApplicationData.Current.LocalFolder.Path}\\localdatabase.db";
        //DatabaseController.Migrate();
        Services.GetRequiredService<DatabaseController>().Migrate();
        //using (var scope = Services.CreateScope())
        //{
        //    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<DataBase_Version3>>();
        //    using var context = factory.CreateDbContext();
        //    context.Database.Migrate(); // 在这里安全地执行迁移
        //}

        var language = Services
            .GetRequiredService<SettingViewModel>()
            .AppConfig.General.LanguageIndex switch
        {
            1 => "en",
            _ => "zhCN",
        };
        Windows.ApplicationModel.Resources.Core.ResourceContext.SetGlobalQualifierValue(
            "Language",
            language
        );

        DeploymentResult result = DeploymentManager.GetStatus();
        if (result.Status is not DeploymentStatus.Ok)
        {
            await Task.Run(() => DeploymentManager.Initialize());
        }

        // If this is the first instance launched, then register it as the "main" instance.
        // If this isn't the first instance launched, then "main" will already be registered,
        // so retrieve it.
        var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

        // If the instance that'message executing the OnLaunched handler right now
        // isn't the "main" instance.
        if (!mainInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            var activatedEventArgs = Microsoft
                .Windows.AppLifecycle.AppInstance.GetCurrent()
                .GetActivatedEventArgs();
            await mainInstance.RedirectActivationToAsync(activatedEventArgs);
            Process.GetCurrentProcess().Kill();
            return;
        }

        var window = Services.GetRequiredService<MainWindow>();
        window.Title = StringsExtension.ResourceLoader.GetString("AppDisplayName");

        var page = Services.GetRequiredService<MainPage>();
        page.ServiceProvider = Services;
        page.OnNavigated();

        window.SetPage<MainPage>(page);

        window.Activate();

        var folders = Services.GetRequiredService<DatabaseController>().MangaFolder_GetAllPaths();

        var factory = Services.GetRequiredService<MangaFactory>();

        factory.GetAllFolders(folders);
        await factory.StartInitial();
    }
}
