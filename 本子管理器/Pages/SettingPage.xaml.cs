using System;

using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage ()
        {
            this.InitializeComponent();

            //SettingFrame.Navigate(typeof(SettingSubPages.FiltedImagesPage));
        }

        private async void RefreshMangaList_1 (object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = await FoldersHelper.GetCoversFolder();
            await Windows.System.Launcher.LaunchFolderAsync(storageFolder);
        }

        private void Button_Click (object sender, RoutedEventArgs e)
        {
            SettingFrame.Navigate(typeof(SettingSubPages.FiltedImagesPage));
        }
    }
}