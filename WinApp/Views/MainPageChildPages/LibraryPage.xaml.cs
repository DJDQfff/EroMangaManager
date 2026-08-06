// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace WinApp.Views.MainPageChildPages;

/// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
public sealed partial class LibraryPage : Page
{
    readonly ObservableCollectionVM viewModel;
    readonly SettingViewModel settingviewmodel;
    readonly DatabaseController databaseController;

    /// <summary>
    /// 构造函数
    /// </summary>
    public LibraryPage(
        ObservableCollectionVM _viewModel,
        SettingViewModel _settingviewmodel,
        DatabaseController _databaseController
    )
    {
        InitializeComponent();
        viewModel = _viewModel;
        settingviewmodel = _settingviewmodel;
        databaseController = _databaseController;
    }

    private async void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { DataContext: MangasGroup group })
        {
            // Remove the folder from the database
            await databaseController.MangaFolder_RemoveSingle(group.FolderPath);
            // Remove the folder from the view model
            viewModel.RemoveFolder(group);
        }
    }

    private async void JumpToBookcaseButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { DataContext: MangasGroup group })
        {
            await App
                .Services.GetRequiredService<MainPage>()
                .NavigateToPage<Bookcase>()
                .OnNavigated(group);
        }
    }

    private void SetAsDefaultBookcaseFolder_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { DataContext: MangasGroup group2 })
        {
            settingviewmodel.AppConfig.General.DefaultBookcaseFolder = group2.FolderPath;
        }
    }

    private async void AddFolder(XamlUICommand sender, ExecuteRequestedEventArgs args)
    {
        var folderPicker = new FolderPicker();

        var handle = WindowNative.GetWindowHandle(App.Services.GetRequiredService<MainWindow>());
        InitializeWithWindow.Initialize(folderPicker, handle);

        folderPicker.FileTypeFilter.Add(".");
        folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
        var selectedRootFolder = await folderPicker.PickSingleFolderAsync();
        var selectedfolderpath = selectedRootFolder?.Path;

        if (selectedfolderpath != null)
        {
            var folderws = new List<string>() { selectedfolderpath };
            if (settingviewmodel.AppConfig.General.WhetherPickSubFolder)
            {
                var fs = Directory.GetDirectories(
                    selectedfolderpath,
                    "*",
                    SearchOption.AllDirectories
                );
                folderws.AddRange(fs);
            }

            var reservedFolders = databaseController.MangaFolder_GetAllPaths();
            foreach (var folder in folderws)
            {
                if (!reservedFolders.Contains(folder))
                {
                    _ = await databaseController.MangaFolder_AddSingle(folder);
                }
            }
            foreach (var folder in folderws)
            {
                if (!viewModel.EnsureAddFolder(folder, out _))
                {
                    await viewModel.StartInitial();
                    //await App.Current.initialStack.StartAsync();
                    //await App.Current.BackgroundCoverSetter.LoopWork3();
                }
                ;
            }
        }
    }

    [RelayCommand]
    private void LocateMangaInFolder(MangasGroup group)
    {
        ExplorerFile.ExplorerSelectFile(group.FolderPath);
    }

    private async void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
    {
        if (sender is Grid { DataContext: MangasGroup group })
        {
            await App
                .Services.GetRequiredService<MainPage>()
                .NavigateToPage<Bookcase>()
                .OnNavigated(group);
        }
    }

    private async void LoadSubDIrectory(object sender, RoutedEventArgs e)
    {
        if (sender is MenuFlyoutItem { DataContext: MangasGroup datacontext })
        {
            var folder = datacontext.FolderPath;
            var chidlfolders = Directory.GetDirectories(folder);

            var reservedFolders = databaseController.MangaFolder_GetAllPaths();

            foreach (var chidlfolder in chidlfolders)
            {
                if (!reservedFolders.Contains(chidlfolder))
                {
                    _ = await databaseController.MangaFolder_AddSingle(chidlfolder);
                }

                if (!viewModel.EnsureAddFolder(chidlfolder, out _))
                {
                    await viewModel.StartInitial();
                }
            }
        }
    }
}
