﻿using System;
using System.Text.Json;
using EroMangaManager.Models;
using EroMangaManager.Helpers;
using static EroMangaManager.MainPage;
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
        public static Manga a;

        public Bookcase ()
        {
            this.InitializeComponent();
            Bookcase_GridView.ItemClick += currentMainPage.TryChangeReadPage;
            ;
        }

        private void GridView_ItemClick (object sender, ItemClickEventArgs e)
        {
            //MainPage.current.MainFrame.Navigate(typeof(ReadPage), storageFile);
            //使用下面这个更好
            this.Frame.Navigate(typeof(EroMangaManager.Pages.ReadPage));
            MainPage.currentMainPage.MainNavigationView.SelectedItem = MainPage.currentMainPage.MainNavigationView.MenuItems[2];
        }

        private async void FilteThisImage_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
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

        private async void RefreshMangaList (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;

            await CoverHelper.ClearCovers();

            MainPage.currentMainPage.listObserver.Initialize();

            button.IsEnabled = true;
        }
    }
}