using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class LibraryPage : Page
    {
        public LibraryPage ()
        {
            this.InitializeComponent();
        }

        private async void addButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFolder folder = await PickHelper.PickFolder();

            if (folder != null)
            {
                MainPage.current.collectionObserver.AddFolder(folder);
            }
        }

        private void removeButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            StorageFolder storageFolder = button.DataContext as StorageFolder;

            MainPage.current.collectionObserver.RemoveFolder(storageFolder);
        }
    }
}