using System;

using EroMangaManager.Helpers;
using EroMangaManager.Models;
using EroMangaManager.Services;
using EroMangaManager.InteractPage;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class Bookcase : Page
    {
        /// <summary>
        /// 构造
        /// </summary>
        public Bookcase ()
        {
            this.InitializeComponent();
        }
        /// <summary>
        /// 点击漫画是，跳转到Read页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_ItemClick_ReadManga (object sender, ItemClickEventArgs e)
        {
            //MainPage.current.MainFrame.Navigate(typeof(ReadPage), storageFile);
            //使用下面这个更好

            MangaBook mangaBook = e.ClickedItem as MangaBook;

            this.Frame.Navigate(typeof(EroMangaManager.Views.ReadPage), mangaBook);

            MainPage.current.MainNavigationView.SelectedItem = MainPage.current.MainNavigationView.MenuItems[2];
        }

        private async void DeleteEroMangaFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            MangaBook eroManga = menuFlyout.DataContext as MangaBook;
            ConfirmDeleteMangaFile confirm = new ConfirmDeleteMangaFile(eroManga);
            var result = await confirm.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    await MainPage.current.collectionObserver.DeleteSingleMangaBook(eroManga);
                    break;

                case ContentDialogResult.Secondary:
                    break;
            }
        }

        private async void TranslateEachMangaName (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;

            try
            {
                await Translator.TranslateAllMangaName();
            }
            catch
            {
            }
            var items = Bookcase_GridView.Items;
            foreach (var item in items)
            {
                var manga = item as MangaBook;
                GridViewItem grid = Bookcase_GridView.ContainerFromItem(item) as GridViewItem;
                var root = grid.ContentTemplateRoot as Grid;
                var run = root.FindName("runtext") as Windows.UI.Xaml.Documents.Run;
                run.Text = manga.TranslatedMangaName;
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

        private async void ViewMangaTag (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyoutItem = sender as MenuFlyoutItem;
            MangaBook mangaBook = menuFlyoutItem.DataContext as MangaBook;
            MangaTagDetail mangaTagDetail = new MangaTagDetail(mangaBook);
            var result = await mangaTagDetail.ShowAsync();
            if (result is ContentDialogResult.Primary)
            {
                await new MangaTagEdit(mangaBook).ShowAsync();
            }
        }

        private async void LaunchFolder (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;
            await Windows.System.Launcher.LaunchFolderAsync(mangaBook.StorageFolder);
        }

        private async void LaunchFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;
            await Windows.System.Launcher.LaunchFileAsync(mangaBook.StorageFile);
        }

        /// <summary>
        /// TODO 不知道为什么，ComboBox的SelectedIndex总是-1
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void SelecteFolderShowComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            var combobox = sender as ComboBox;
            int index = combobox.SelectedIndex;
            var folder = MainPage.current.collectionObserver.FolderList[index];
            MainPage.current.collectionObserver.Initialize(folder);
        }

        private void Page_Loaded (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            MainPage.current.bookcase = this;
        }
    }
}