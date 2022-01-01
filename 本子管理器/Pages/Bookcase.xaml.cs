using System;

using EroMangaManager.Helpers;
using EroMangaManager.Models;

using Windows.UI.Xaml.Controls;

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
            Bookcase_GridView.ItemClick += ReadPage.TryChangeManga;
        }

        private void GridView_ItemClick (object sender, ItemClickEventArgs e)
        {
            //MainPage.current.MainFrame.Navigate(typeof(ReadPage), storageFile);
            //使用下面这个更好
            this.Frame.Navigate(typeof(EroMangaManager.Pages.ReadPage));
            MainPage.current.MainNavigationView.SelectedItem = MainPage.current.MainNavigationView.MenuItems[2];
        }

        private async void FilteThisImage_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            MangaBook eroManga = menuFlyout.DataContext as MangaBook;
            await new ContentDialog() { CloseButtonText = eroManga.MangaName }.ShowAsync();
        }

        private async void DeleteEroManga (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            MangaBook eroManga = menuFlyout.DataContext as MangaBook;
            await MainPage.current.collectionObserver.DeleteSingleMangaBook(eroManga);
        }

        private async void TranslateEachMangaName (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;

            try
            {
                await Translater.TranslateAllMangaName();
            }
            catch
            {
            }

            button.IsEnabled = true;
        }

        private async void RefreshMangaList (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;
            try
            {
                await CoverHelper.ClearCovers();

                MainPage.current.collectionObserver.Initialize();
            }
            finally
            {
            }

            button.IsEnabled = true;
        }
    }
}