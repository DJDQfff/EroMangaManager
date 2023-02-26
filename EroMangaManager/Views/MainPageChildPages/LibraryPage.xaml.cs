﻿using System;
using System.Collections.Generic;

using EroMangaManager.Models;
using EroMangaManager.ViewModels;

using MyUWPLibrary;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

using static MyUWPLibrary.StorageItemPicker;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class LibraryPage : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public LibraryPage ()
        {
            this.InitializeComponent();
        }

        private async void addButton_Click (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;
            StorageFolder folder = await OpenSingleFolderAsync();

            if (folder != null)
            {
                List<StorageFolder> folders;
                (folders, _) = await folder.GetAllStorageItems();
                folders.Add(folder);//得把文件夹自身也加入扫描类中
                foreach (var f in folders)
                {
                    try
                    {
                        await App.Current.collectionObserver.AddFolder(f);
                    }
                    catch (Exception)
                    {
                        // 出错
                    }
                }
            }

            button.IsEnabled = true;
        }

        private void removeButton_Click (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var storageFolder = button.DataContext as MangasFolder;

            App.Current.collectionObserver.RemoveFolder(storageFolder);
        }

        private async void LauncherFolder (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var mf = button.DataContext as MangasFolder;

            await Windows.System.Launcher.LaunchFolderAsync(mf.StorageFolder);
        }

        private void JumpToBookcase_Click (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var datacontext = button.DataContext as MangasFolder;

            MainPage.Current.MainFrame.Navigate(typeof(Bookcase) , datacontext);
        }

        private void SetAsDefaultBookcaseFolder_Click (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var datacontext = button.DataContext as MangasFolder;

            ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.DefaultBookcaseFolder.ToString()] = datacontext.FolderPath;
        }
    }
}