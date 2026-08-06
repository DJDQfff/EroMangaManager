// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
using Microsoft.EntityFrameworkCore;
using Microsoft.Windows.ApplicationModel.WindowsAppRuntime;
using Microsoft.Windows.AppNotifications.Builder;

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

        //无其他依赖项
        services.AddTransient<ClipboardHelper>();
        services.AddSingleton<UnoLibrary.Services.ZipEntryHelper>();
        services.AddTransient<MangaStreamProvider>();
        services.AddSingleton<Exporter>();
        services.AddSingleton<MangaFileIO>();
        services.AddSingleton<SettingViewModel>();
        services.AddSingleton<CoverSetter>();
        services.AddSingleton<ObservableCollectionVM>();
        services.AddTransient<ISettingFilePath, WinUISetting>();
        //依赖前面的
        services.AddSingleton<Translator>();
        services.AddSingleton<StorageOperation>();
        services.AddSingleton<DialogHelper>();
        services.AddSingleton<StorageFolderHelper>();
        services.AddSingleton<CoverHelper>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MangaFactory>();
        services.AddSingleton<SettingViewModel>();
        services.AddTransient<TagCategorySelect>();
        services.AddSingleton<ManageTagsViewModel2>();
        services.AddTransient<MangaOperationViewModel>();
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
        #region 快速执行

#if DEBUG
        //await Windows.System.Launcher.LaunchFolderPathAsync(LocalFolder);
#endif

        Services.GetRequiredService<CoverSetter>().SetCover += async manga =>
        {
            if (manga.CoverUri.EndsWith(".svg"))
            {
                manga.CoverUri = await Services
                    .GetRequiredService<MangaFactory>()
                    .GetCoverFile(manga);
            }
        };
        Services.GetRequiredService<CoverSetter>().MangaInfo += async manga =>
        {
            if (manga.FileSize == 0)
            {
                await Services.GetRequiredService<MangaFileIO>().LoadMangaInfo(manga);
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

        #region 事件赋值

        var globalviewmodel = Services.GetRequiredService<ObservableCollectionVM>();

        globalviewmodel.ErrorZipEvent += str =>
        {
            var appNotification = new AppNotificationBuilder()
                .AddText($"{str}\r{StringsExtension.ResourceLoader.GetString("ErrorString1")}")
                .BuildNotification();
            AppNotificationManager.Default.Show(appNotification);
        };
        globalviewmodel.WorkDoneEvent += Toast;
        globalviewmodel.WorkFailedEvent += Toast;
        globalviewmodel.AccessDeniedEvent += ToastAccessDenied;

        #endregion 事件赋值

        InitializeGlobalViewModel();

        DeploymentResult result = DeploymentManager.GetStatus();
        if (result.Status is not DeploymentStatus.Ok)
        {
            await Task.Run(() => DeploymentManager.Initialize());
        }

        #endregion 快速执行

        // If this is the first instance launched, then register it as the "main" instance.
        // If this isn't the first instance launched, then "main" will already be registered,
        // so retrieve it.
        var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("main");

        // If the instance that's executing the OnLaunched handler right now
        // isn't the "main" instance.
        if (!mainInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            var activatedEventArgs = Microsoft
                .Windows.AppLifecycle.AppInstance.GetCurrent()
                .GetActivatedEventArgs();
            await mainInstance.RedirectActivationToAsync(activatedEventArgs);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return;
        }

        var window = Services.GetRequiredService<MainWindow>();
        // 格式："类库名称/资源文件名" (如果资源文件是默认的 Resources.resw，则省略 .resw)
        window.Title = Windows
            .ApplicationModel.Resources.ResourceLoader.GetForViewIndependentUse()
            .GetString("AppDisplayName");

        var page = Services.GetRequiredService<MainPage>();
        page.ServiceProvider = Services;
        page.OnNavigated();

        window.SetPage<MainPage>(page);

        window.Activate();

        #region 需要后台执行

        await globalviewmodel.StartInitial();

        //await Current.BackgroundCoverSetter.LoopWork3();

        //GlobalViewModel.InitialEachFoldersInOrder();

        #endregion 需要后台执行
    }

    /// <summary>
    /// 初始化文件夹目录
    /// </summary>
    private void InitializeGlobalViewModel()
    {
        var folders = Services.GetRequiredService<DatabaseController>().MangaFolder_GetAllPaths();

        // 这个是以前设计的会把默认书架放第一个加载
        //var defaultpath = AppConfig.AppConfig.General.DefaultBookcaseFolder;
        //var f = folders.SingleOrDefault(x => x == defaultpath);
        //if (f != null)
        //{
        //    folders.Remove(f);
        //    folders.Insert(0, f);
        //}
#if DEBUG_TESTFOLDER
        folders = [@"E:\test"];
#endif
        var viewmodel = Services.GetRequiredService<ObservableCollectionVM>();
        Services.GetRequiredService<MangaFactory>().GetAllFolders(viewmodel, folders);
        viewmodel.InitialGroup += Services.GetRequiredService<MangaFactory>().InitialGroup2;
    }

    private void Toast(string message)
    {
        var appNotification = new AppNotificationBuilder().AddText(message).BuildNotification();
        AppNotificationManager.Default.Show(appNotification);
    }

    private void ToastAccessDenied()
    {
        var denied = StringsExtension.ResourceLoader.GetString("AccessDenied");
        var appNotification = new AppNotificationBuilder().AddText(denied).BuildNotification();
        AppNotificationManager.Default.Show(appNotification);
    }
}
