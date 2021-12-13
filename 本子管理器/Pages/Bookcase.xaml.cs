using System;
using System.Text.Json;
using EroMangaManager.Models;
using System.Linq;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using BaiduTranslate;
using BaiduTranslate.Models;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class Bookcase : Page
    {
        public Bookcase ()
        {
            this.InitializeComponent();
        }

        private void GridView_ItemClick (object sender, ItemClickEventArgs e)
        {
            var a = (Manga) e.ClickedItem;

            //MainPage.current.MainFrame.Navigate(typeof(ReadPage), storageFile);
            //使用下面这个更好
            this.Frame.Navigate(typeof(ReadPage), a);

            MainPage.current.MainNavigationView.SelectedItem = MainPage.current.MainNavigationView.MenuItems[2];
        }

        private async void MenuFlyoutItem_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            Manga eroManga = menuFlyout.DataContext as Manga;
            await new ContentDialog() { CloseButtonText = eroManga.MangaName }.ShowAsync();
        }

        private async void DeleteEroManga (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            Manga eroManga = menuFlyout.DataContext as Manga;
            await eroManga.RemoveFile();
        }

        private async void TranslateEachMangaName (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Translater.TranslateAllMangaName();
        }
    }
}