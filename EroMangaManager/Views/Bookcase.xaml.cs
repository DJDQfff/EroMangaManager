using System;

using EroMangaManager.Helpers;
using EroMangaManager.Models;
using EroMangaManager.InteractPage;
using Windows.UI.Xaml.Controls;
using EroMangaManager.ViewModels;
using Windows.Storage;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Resources;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class Bookcase : Page
    {
        internal MangasFolder BindMangasFolder { get; }
        /// <summary>
        /// 构造
        /// </summary>
        public Bookcase ()
        {
            this.InitializeComponent();
        }
        internal Bookcase (MangasFolder mangasFolder) : this()
        {
            BindMangasFolder= mangasFolder;
        }
        /// <summary>
        /// 点击漫画是，跳转到Read页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_ItemClick_ReadManga (object sender, ItemClickEventArgs e)
        {
            MangaBook mangaBook = e.ClickedItem as MangaBook;

            MainPage.current.MainFrame.Navigate(typeof(EroMangaManager.Views.ReadPage), mangaBook);

            MainPage.current.MainNavigationView.SelectedItem = MainPage.current.MainNavigationView.MenuItems[2];
        }

        private async void DeleteSourceMangaFile (object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

        private async void ExportPDF (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFile storageFile = await MyUWPLibrary.StorageItemPicker.SaveFileAsync(".pdf");
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;

              await  Exporter.ExportAsPDF(mangaBook , storageFile);

            // TODO 存在线程bug

            string done = MainPage.current.resourceLoader.GetString("ExportDone");
            MainPage.current.collectionObserver.WorkDone(done);

        }
    }
}