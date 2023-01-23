using System;

using EroMangaManager.Helpers;
using EroMangaManager.Models;
using EroMangaManager.ViewModels;
using EroMangaManager.Views.InteractPages;

using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Bookcase : Page
    {
        internal MangasFolder BindMangaFolder { set; get; }

        /// <summary>
        ///
        /// </summary>
        public Bookcase ()
        {
            this.InitializeComponent();
            App.Current.bookcaseContainer = this;
        }

        /// <summary>
        /// 导航时，传入要绑定的数据
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            switch(e.Parameter)
            {
                case MangasFolder mangasFolder:
                    Bookcase_GridView.ItemsSource = mangasFolder.MangaBooks;
                    break;



            }
        }

        //TODO 因为原来的Bookcase被拆分为Bookcase和Bookcase两个类，所以这个方法现在有bug
        private async void RefreshMangaList (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;

            await CoverHelper.ClearCovers();

            var folder = MyUWPLibrary.AccestListHelper.GetAvailableFutureFolder().Result.ToArray();

            App.Current.collectionObserver.Initialize(folder);

            button.IsEnabled = true;
        }

        //TODO 因为原来的Bookcase被拆分为Bookcase和Bookcase两个类，所以这个方法现在有bug
        private async void TranslateEachMangaName (object sender , Windows.UI.Xaml.RoutedEventArgs e)
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

        /// <summary>
        /// 点击漫画是，跳转到Read页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridView_ItemClick_ReadManga (object sender , ItemClickEventArgs e)
        {
            MangaBook mangaBook = e.ClickedItem as MangaBook;

            MainPage.current.MainFrame.Navigate(typeof(ReadPage) , mangaBook);

            MainPage.current.MainNavigationView.SelectedItem = MainPage.current.MainNavigationView.MenuItems[2];
        }

        private async void DeleteSourceMangaFile (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            MangaBook eroManga = menuFlyout.DataContext as MangaBook;

            await Helpers.StorageHelper.DeleteSourceFile(eroManga);
        }

        private async void ViewMangaTag (object sender , Windows.UI.Xaml.RoutedEventArgs e)
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

        private async void LaunchFolder (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;
            await Windows.System.Launcher.LaunchFolderAsync(mangaBook.StorageFolder);
        }

        private async void LaunchFile (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;
            await Windows.System.Launcher.LaunchFileAsync(mangaBook.StorageFile);
        }

        private async void ExportPDF (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            StorageFile storageFile = await MyUWPLibrary.StorageItemPicker.SaveFileAsync(".pdf");
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;

            await Exporter.ExportAsPDF(mangaBook , storageFile);

            string done = ResourceLoader.GetForCurrentView("StringResources").GetString("ExportDone");
            App.Current.collectionObserver.WorkDone(done);
        }

    }
}