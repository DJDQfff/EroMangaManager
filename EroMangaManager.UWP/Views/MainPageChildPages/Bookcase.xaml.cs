using System;

using EroMangaManager.UWP.Helpers;
using EroMangaManager.Core.Models;
using EroMangaManager.UWP.ViewModels;
using EroMangaManager.UWP.Views.ContentDialogPages;

using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using MyLibrary.UWP;
// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Bookcase : Page
    {
        private MangasFolder _data;

        internal MangasFolder BindMangasFolder
        {
            set
            {
                _data = value;
                Bookcase_GridView.ItemsSource = value.MangaBooks;
                Bookcase_HintTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            get
            {
                return _data;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Bookcase ()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 导航时，传入要绑定的数据
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MainPage.Current.MainNavigationView.SelectedItem = MainPage.Current.MainNavigationView.MenuItems[0];

            switch (e.Parameter)
            {
                case MangasFolder mangasFolder:
                    BindMangasFolder = mangasFolder;
                    break;
            }
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

            MainPage.Current.MainFrame.Navigate(typeof(ReadPage) , mangaBook);

            MainPage.Current.ReadItem.Visibility = Windows.UI.Xaml.Visibility.Visible;

            MainPage.Current.MainNavigationView.SelectedItem = MainPage.Current.MainNavigationView.MenuItems[2];
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
            await Windows.System.Launcher.LaunchFolderPathAsync(mangaBook.FolderPath);
        }

        private async void LaunchFile (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var mangaBook = (sender as MenuFlyoutItem).DataContext as MangaBook;
            await Windows.System.Launcher.LaunchFileAsync(await mangaBook.FilePath.GetStorageFile());
        }

        private async void ExportPDF (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            MangaBook mangaBook = menuFlyout.DataContext as MangaBook;

            StorageFile storageFile = await MyLibrary.UWP.StorageItemPicker.SaveFileAsync(".pdf");

            await Exporter.ExportAsPDF(mangaBook , storageFile);

            string done = ResourceLoader.GetForCurrentView("StringResources").GetString("ExportDone");
            App.Current.GlobalViewModel.WorkDone(done);
        }

        private void Order (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            BindMangasFolder.SortMangaBooks(x => x.FileSize);
        }

        private async void Rename (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            MenuFlyoutItem menuFlyout = sender as MenuFlyoutItem;
            MangaBook eroManga = menuFlyout.DataContext as MangaBook;

            RenameDialog renameDialog = new RenameDialog(eroManga);
            var result = await renameDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var text = renameDialog.NewDisplayName;
                // text是否合法由对话框保证
                await App.Current.GlobalViewModel.ReNameSingleMangaBook(eroManga , text);
            }
            else
            {
            }
        }
    }
}