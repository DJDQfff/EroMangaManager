using System;
using System.Collections.Generic;
using System.Linq;

using EroMangaManager.Core.ViewModels;
using EroMangaManager.UWP.Models;

using MyLibrary.UWP;

using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Controls;

using static MyLibrary.UWP.StorageItemPicker;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.MainPageChildPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class LibraryPage : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LibraryPage()
        {
            this.InitializeComponent();
        }

        private async void Button_AddFolder_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            StorageFolder selectedRootFolder = await OpenSingleFolderAsync();

            if (selectedRootFolder != null)
            {
                var libraryfolders = App.Current.AppConfig.LibraryFolders;
                    if (!libraryfolders.Contains(selectedRootFolder.Path))
                    {
                        List<StorageFolder> folders = await selectedRootFolder.GetAllStorageFolder();
                        folders.Insert(0, selectedRootFolder);//得把文件夹自身也加入扫描类中
                        folders.Sort((x, y) => x.Path.CompareTo(y.Path));

                        var array = App.Current.AppConfig.LibraryFolders;
                        List<string> strings = new List<string>(array);
                        foreach (var f in folders)
                        {
                            strings.Add(f.Path);
                            StorageApplicationPermissions.FutureAccessList.Add(f);

                            var a = App.Current.GlobalViewModel.AddFolder(f.Path);
                            await ModelFactory.InitialMangasFolder(a, f);
                        }

                        App.Current.AppConfig.LibraryFolders = strings.Distinct().ToArray();
                    

                }
            }

            button.IsEnabled = true;
        }

        private async void RemoveFolderButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (list.SelectedItem is MangasFolder storageFolder)
            {
                var librarys= App.Current.AppConfig.LibraryFolders;
                var strings = new List<string>(librarys);
                strings.Remove(storageFolder.FolderPath);
                App.Current.AppConfig.LibraryFolders= strings.ToArray();

              await  MyLibrary.UWP.AccestListHelper.RemoveFolder(storageFolder.FolderPath);

                App.Current.GlobalViewModel.RemoveFolder(storageFolder);
            }
        }

        private async void LauncherFolder(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (list.SelectedItem is MangasFolder mf)
            {
                await Windows.System.Launcher.LaunchFolderPathAsync(mf.FolderPath);
            }
        }

        private void JumpToBookcaseButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (list.SelectedItem is MangasFolder datacontext)
            {
                if (!datacontext.IsInitialing)
                {
                    // TODO 我也知道这个留着是干嘛的
                }
                MainPage.Current.MainFrame.Navigate(typeof(Bookcase), datacontext);
            }
        }

        private void SetAsDefaultBookcaseFolder_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (list.SelectedItem is MangasFolder datacontext)
            {
                var folders = App.Current.AppConfig.LibraryFolders ;
                var list = new List<string>(folders);

                var defaultpath= datacontext.FolderPath;
                list.Remove(defaultpath);
                list.Insert(0, defaultpath);
                App.Current.AppConfig.LibraryFolders = list.ToArray();
            }
        }

        private void ToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            App.Current.AppConfig.IsEmptyFolderShow = toggleSwitch.IsOn;
        }

        private void ToggleSwitch_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            var a = App.Current.AppConfig.IsEmptyFolderShow;
            toggleSwitch.IsOn = a;
        }
    }
}