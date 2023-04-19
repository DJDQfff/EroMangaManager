using System;
using System.Collections.Generic;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Helpers;
using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.Views.MainPageChildPages;

using GroupedItemsLibrary.ViewModels;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.FunctionChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FindSameManga : Page
    {
        private ItemsGroupsViewModel<string, MangaBook, RepeatMangaBookGroup> mangaBookViewModel;

        private List<MangaBook> mangaBooks;

        /// <summary>
        ///
        /// </summary>
        public FindSameManga()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            mangaBooks = e.Parameter as List<MangaBook>;
            mangaBookViewModel = new ItemsGroupsViewModel<string, MangaBook, RepeatMangaBookGroup>(mangaBooks, n => n.MangaName);
            listView.ItemsSource = mangaBookViewModel.RepeatPairs;
        }

        private async void DeleteFileClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
           var manga=button.DataContext as MangaBook;

            await Helpers.StorageHelper.DeleteSourceFile(manga);

            mangaBookViewModel.DeleteStorageFileInRootObservable(manga);
        }

        private async void OpenMangaClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var manga = button.DataContext as MangaBook;

            await WindowHelper.ShowNewReadPageWindow(manga);
        }

    }
}