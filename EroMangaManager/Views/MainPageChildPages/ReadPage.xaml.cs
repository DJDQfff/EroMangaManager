﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using EroMangaManager.Models;
using EroMangaManager.ViewModels;

using SharpCompress.Archives;

using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static EroMangaManager.Helpers.ZipEntryHelper;
using static EroMangaManager.Models.FolderEnum;
using static MyStandard20Library.HashComputer;
using static MyUWPLibrary.StorageFolderHelper;
using static MyUWPLibrary.StorageItemPicker;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class ReadPage : Page
    {
        private MangaBook currentManga = null;

        /// <summary>
        /// 当前ReaderViewModel
        /// </summary>
        public ReaderVM currentReader = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReadPage ()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 导航前
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            Debug.WriteLine("OnNavigatedTo事件开始");

            var mangaBook = e.Parameter as MangaBook;

            await TryChangeManga(mangaBook);
        }

        /// <summary>
        /// 修改阅读漫画
        /// </summary>
        /// <param name="manga"></param>
        /// <returns></returns>
        public async Task TryChangeManga (MangaBook manga)
        {
            if (manga == null)                          // 传入null，直接跳过
            {
                return;
            }
            if (manga != currentManga)                  // 传入新漫画，则设置新源
            {
                await SetNewSource(manga);
                Debug.WriteLine(this.GetHashCode());
            }
            else                                       // 未传入新漫画，不作变动。这个其实不用写了
            {
                //Do Nothing
            }

            async Task SetNewSource (MangaBook newmanga)
            {
                currentManga = newmanga;
                var oldreader = currentReader;
                currentReader = new ReaderVM(manga);
                oldreader?.Dispose();

                FLIP.ItemsSource = currentReader.bitmapImages;

                var result = ApplicationData.Current.LocalSettings.Values["IsFilterImageOn"] ?? false;

                var isfilterimage = (bool) result;
                await currentReader.SelectEntriesAsync(isfilterimage);
            }
        }

        #region 数据绑定到已解码的BitmapImage

        private async void FilteThisImage2_Click (object sender , RoutedEventArgs e)
        {
            var bitmap = FLIP.SelectedItem as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            var index = currentReader.bitmapImages.IndexOf(bitmap);
            var entry = currentReader.zipArchiveEntries[index];
            currentReader.zipArchiveEntries.Remove(entry);
            currentReader.bitmapImages.Remove(bitmap);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await HashManager.Add(hash , length);

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path , hash + ".jpg");
            entry.WriteToFile(path);
        }

        /// <summary> 此图片另存为 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>

        private async void SaveImageAs2_Click (object sender , RoutedEventArgs e)
        {
            var bitmap = FLIP.SelectedItem as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            var index = currentReader.bitmapImages.IndexOf(bitmap);
            var entry = currentReader.zipArchiveEntries[index];
            StorageFile storageFile = await SavePictureAsync();
            if (storageFile != null)
            {
                using (Stream stream = await storageFile.OpenStreamForWriteAsync())
                {
                    Stream stream1 = entry.OpenEntryStream();
                    await stream1.CopyToAsync(stream);
                }
            }
        }

        #endregion 数据绑定到已解码的BitmapImage

        #region 数据绑定到压缩文件类的每个压缩入口Entry

        [Obsolete]
        private async void FLIP_SelectionChangedNew (object sender , SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"SelectionChanged事件开始增加个数：{e.AddedItems.Count}移除个数：{e.RemovedItems.Count}");

            FlipView flipView = sender as FlipView;

            var entry = flipView.SelectedItem as IArchiveEntry;

            FlipViewItem item = flipView.ContainerFromItem(entry) as FlipViewItem;
            if (item is null)
            {
                return;
            }

            var root = item.ContentTemplateRoot as Grid;

            var image = root.FindName("image") as Image;
            image.Source = await ShowEntryAsync(entry);
            Debug.WriteLine("SelectionChanged事件结束");
        }

        /// <summary> 切换图,这个不在使用 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        [Obsolete]
        private async void FLIP_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            int c = e.AddedItems.Count;

            if (c == 0)                 // 从集合中移出项，触发两次此事件，第一次additems个数为0，第二次additems不为0
                return;

            var entry = e.AddedItems[0] as IArchiveEntry;

            FlipViewItem item = FLIP.ContainerFromItem(entry) as FlipViewItem;
            var root = item.ContentTemplateRoot as Grid;

            //var sc = root.Children[0] as ScrollViewer;
            //var image = sc.Content as Image;

            var image = root.FindName("image") as Image;
            image.Source = await ShowEntryAsync(entry);
        }

        /// <summary> 添加此图片到过滤图库 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        [Obsolete]
        private async void FilteThisImage_Click (object sender , RoutedEventArgs e)
        {
            var entry = FLIP.SelectedItem as IArchiveEntry;

            currentReader.zipArchiveEntries.Remove(entry);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await HashManager.Add(hash , length);

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path , hash + ".jpg");
            entry.WriteToFile(path);
        }

        [Obsolete]
        private async void SaveImageAs_Click (object sender , RoutedEventArgs e)
        {
            var entry = FLIP.SelectedItem as IArchiveEntry;
            StorageFile storageFile = await SavePictureAsync();
            if (storageFile != null)
            {
                Stream stream = await storageFile.OpenStreamForWriteAsync();
                Stream stream1 = entry.OpenEntryStream();
                await stream1.CopyToAsync(stream);
            }
        }

        #endregion 数据绑定到压缩文件类的每个压缩入口Entry

        private void FLIP_Tapped (object sender , Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var a = ReadPageButtonGroup.Visibility;
            if (a == Visibility.Collapsed)
            {
                ReadPageButtonGroup.Visibility = Visibility.Visible;
            }
            else
            {
                ReadPageButtonGroup.Visibility = Visibility.Collapsed;
            }
        }

        private void AppBarToggleButton_Checked (object sender , RoutedEventArgs e)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            applicationView.TryEnterFullScreenMode();
            MainPage.Current.MainNavigationView.IsPaneToggleButtonVisible = false;

            ReadPageButtonGroup.Visibility = Visibility.Collapsed;
        }

        private void AppBarToggleButton_Unchecked (object sender , RoutedEventArgs e)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            applicationView.ExitFullScreenMode();
            MainPage.Current.MainNavigationView.IsPaneToggleButtonVisible = true;
        }
    }
}