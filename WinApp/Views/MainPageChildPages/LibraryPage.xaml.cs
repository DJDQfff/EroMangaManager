// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class LibraryPage : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LibraryPage()
        {
            InitializeComponent();
        }

        private void RemoveFolderButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem).DataContext is MangasGroup storageFolder)
            {
                DatabaseController.MangaFolder_RemoveSingle(storageFolder.FolderPath);

                App.Current!.GlobalViewModel.RemoveFolder(storageFolder);
            }
        }

        private void JumpToBookcaseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem).DataContext is MangasGroup datacontext)
            {
                MainPage.Current.MainFrame.Navigate(typeof(Bookcase), datacontext);
            }
        }

        private void SetAsDefaultBookcaseFolder_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem).DataContext is MangasGroup datacontext)
            {
                App.Current.AppConfig.AppConfig.General.DefaultBookcaseFolder =
                    datacontext.FolderPath;
            }
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;

            App.Current.AppConfig.AppConfig.General.IsEmptyFolderShow = toggleSwitch.IsOn;
        }

        private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            var toggleSwitch = sender as ToggleSwitch;

            toggleSwitch.IsOn = App.Current.AppConfig.AppConfig.General.IsEmptyFolderShow;
        }

        private async void AddFolder(XamlUICommand sender, ExecuteRequestedEventArgs args)
        {
            var folderPicker = new FolderPicker();

            var handle = WindowNative.GetWindowHandle(App.Current.MainWindow);
            InitializeWithWindow.Initialize(folderPicker, handle);

            folderPicker.FileTypeFilter.Add(".");
            folderPicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            var selectedRootFolder = await folderPicker.PickSingleFolderAsync();
            var selectedfolderpath = selectedRootFolder?.Path;

            if (selectedfolderpath != null)
            {
                var folderws = new List<string>() { selectedfolderpath };
                if (App.Current.AppConfig.AppConfig.General.WhetherPickSubFolder)
                {
                    var fs = Directory.GetDirectories(selectedfolderpath, "*", SearchOption.AllDirectories);
                    folderws.AddRange(fs);
                }

                var reservedFolders = DatabaseController.MangaFolder_GetAllPaths();
                foreach (var folder in folderws)
                {
                    if (!reservedFolders.Contains(folder))
                    {
                        _ = DatabaseController.MangaFolder_AddSingle(folder);
                    }
                }
                foreach (var folder in folderws)
                {
                    if (!App.Current.GlobalViewModel.EnsureAddFolder(folder, out _))
                    {
                        await App.Current.GlobalViewModel.StartInitial();
                        //await App.Current.initialStack.StartAsync();
                        //await App.Current.BackgroundCoverSetter.LoopWork3();
                    }
                    ;
                }
            }
        }

        private void OpenFolderItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem).DataContext is MangasGroup datacontext)
            {
                MangaCommands.Instance.LocateInExplorer.Execute(datacontext.FolderPath);
            }
        }

        private void Grid_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if ((sender as Grid).DataContext is MangasGroup datacontext)
            {
                MainPage.Current.MainFrame.Navigate(typeof(Bookcase), datacontext);
            }
        }

        private async void LoadSubDIrectory(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuFlyoutItem).DataContext is MangasGroup datacontext)
            {
                var folder = datacontext.FolderPath;
                var chidlfolders = Directory.GetDirectories(folder);

                var reservedFolders = DatabaseController.MangaFolder_GetAllPaths();

                foreach (var chidlfolder in chidlfolders)
                {
                    if (!reservedFolders.Contains(chidlfolder))
                    {
                        _ = DatabaseController.MangaFolder_AddSingle(chidlfolder);
                    }

                    if (!App.Current.GlobalViewModel.EnsureAddFolder(chidlfolder, out _)
                    )
                    {
                        await App.Current.GlobalViewModel.StartInitial();
                    }
                }
            }
        }
    }
}