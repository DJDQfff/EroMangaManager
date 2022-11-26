using System;

using EroMangaManager.Helpers;
using static MyUWPLibrary.StorageItemPicker;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class LibraryPage : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LibraryPage ()
        {
            this.InitializeComponent();
        }

        private async void addButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            StorageFolder folder = await OpenSingleFolderAsync();

            if (folder != null)
            {
                try
                {
                    await MainPage.current.collectionObserver.AddFolder(folder);
                }
                catch (Exception)
                {
                    // 出错
                }
            }

            button.IsEnabled = true;
        }

        private void removeButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            StorageFolder storageFolder = button.DataContext as StorageFolder;

            MainPage.current.collectionObserver.RemoveFolder(storageFolder);
        }
    }
}