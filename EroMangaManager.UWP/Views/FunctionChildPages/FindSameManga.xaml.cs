﻿using System.Collections.Generic;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.Views.MainPageChildPages;

using GroupedItemsLibrary.ViewModels;

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

        private MangaBook SelectedMangaBook;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Helpers.StorageHelper.DeleteSourceFile(SelectedMangaBook);

            mangaBookViewModel.DeleteStorageFileInRootObservable(SelectedMangaBook);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            MainPage.Current.MainFrame.Navigate(typeof(ReadPage), SelectedMangaBook);

            MainPage.Current.MainNavigationView.SelectedItem = MainPage.Current.MainNavigationView.MenuItems[2];
        }

        private void mangalist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listview = sender as ListView;
            var book = listview.SelectedItem as MangaBook;
            SelectedMangaBook = book;
        }
    }
}