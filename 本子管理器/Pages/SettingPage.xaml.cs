using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EroMangaManager.Helpers;
using Windows.Storage;
using System.Threading.Tasks;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage ()
        {
            this.InitializeComponent();
        }


        private async void AppBarButton_Click (object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = await CoversFolder.Get();
            var files = await storageFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }

        }

        private async void AppBarButton_Click_1 (object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = await CoversFolder.Get();
            await Windows.System.Launcher.LaunchFolderAsync(storageFolder);
        }
    }
}
