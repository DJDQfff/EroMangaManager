using System;
using System.Collections.Generic;

using EroMangaManager.Core.ViewModels;
using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.SettingEnums;

using MyLibrary.UWP;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

        private async void addButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            StorageFolder folder = await OpenSingleFolderAsync();


            if (folder != null)
            {
                if (!App.Current.GlobalViewModel.StorageFolders.Contains(folder.Path))
                {
                List<StorageFolder> folders;
                folders = await folder.GetAllStorageFolder();
                folders.Add(folder);//得把文件夹自身也加入扫描类中
                App.Current.storageItemManager.AddTokenRange(folders);

                foreach (var f in folders)
                {
                        var a = App.Current.GlobalViewModel.AddFolder(f.Path);

                        await ModelFactory.InitialMangasFolder(a, f);

                }

                }

            }

            button.IsEnabled = true;
        }

        private void removeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var storageFolder = list.SelectedItem as MangasFolder;
            if (storageFolder != null)
            {
                App.Current.storageItemManager.RemoveToken(storageFolder.FolderPath);
                App.Current.GlobalViewModel.RemoveFolder(storageFolder);
            }
        }

        private async void LauncherFolder(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var mf = list.SelectedItem as MangasFolder;
            if (mf != null)
            {
                await Windows.System.Launcher.LaunchFolderPathAsync(mf.FolderPath);
            }
        }

        private void JumpToBookcase_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var datacontext = list.SelectedItem as MangasFolder;
            if (datacontext != null)
            {
                if (!datacontext.IsInitialing)
                {
                }
                MainPage.Current.MainFrame.Navigate(typeof(Bookcase), datacontext);
            }
        }

        private void SetAsDefaultBookcaseFolder_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var datacontext = list.SelectedItem as MangasFolder;
            if (datacontext != null)
            {
                ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.DefaultBookcaseFolder.ToString()] = datacontext.FolderPath;
            }
        }

        private void ToggleSwitch_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.IsEmptyFolderShow.ToString()] = toggleSwitch.IsOn;
        }

        private void ToggleSwitch_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            var a = (bool)(ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.IsEmptyFolderShow.ToString()] ?? false);
            toggleSwitch.IsOn = a;
        }
    }
}