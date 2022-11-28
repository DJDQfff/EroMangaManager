﻿using System;

using EroMangaManager.Models;
using EroMangaManager.Views;
using EroMangaManager.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Resources;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace EroMangaManager
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class MainPage : Page
    {
        public Bookcase bookcase;
        /// <summary>
        /// CollectionObserver实例
        /// </summary>
        public ObservableCollectionVM collectionObserver { get; }
        /// <summary>
        /// MainPage的单一实例
        /// </summary>
        public static MainPage current { set; get; }
        /// <summary>
        /// 系统resw解析实例
        /// </summary>
        public ResourceLoader resourceLoader { get; } = ResourceLoader.GetForCurrentView();
        /// <summary>
        /// 
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            var folder = MyUWPLibrary.AccestListHelper.GetAvailableFutureFolder().Result.ToArray();
            collectionObserver = new ObservableCollectionVM(folder);

            collectionObserver.ErrorZipEvent += (string str) =>
              {
                  new ToastContentBuilder()
                    .AddText($"{str}\r{resourceLoader.GetString("ErrorString1")}")
                    .Show();
              };

            current = this;
        }
        /// <summary>
        /// 导航前
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainFrame.Navigate(typeof(Bookcase));
        }

        private void MainNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Type type = null;

            if (args.IsSettingsInvoked)
                type = typeof(SettingPage);
            else 
                switch (args.InvokedItemContainer.Name)
                {
                    case nameof(BookcaseItem):
                        type = typeof(Bookcase);
                        break;

                    case nameof(ListItem):
                        type = typeof(LibraryPage);
                        break;

                    case nameof(ReadItem):
                        type = typeof(ReadPage);
                        break;
                }

            if (!type.Equals(MainFrame.CurrentSourcePageType))
            {
                MainFrame.Navigate(type);
            }
        }
    }
}