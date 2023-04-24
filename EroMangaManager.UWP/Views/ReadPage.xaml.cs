﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using EroMangaDB;
using EroMangaDB.Entities;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.SettingEnums;
using EroMangaManager.UWP.ViewModels;

using SharpCompress.Archives;

using SharpConfig;

using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static EroMangaDB.BasicController;
using static EroMangaManager.UWP.Helpers.ZipEntryHelper;
using static EroMangaManager.UWP.SettingEnums.FolderEnum;
using static EroMangaManager.UWP.SettingEnums.General;
using static MyLibrary.Standard20.HashComputer;
using static MyLibrary.UWP.StorageFolderHelper;
using static MyLibrary.UWP.StorageItemPicker;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views
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
        /// 任务取消令牌，留着给Select方法用的，但是有bug未解决
        /// </summary>
        public CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 构造函数
        /// </summary>
        public ReadPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 导航前
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Debug.WriteLine("OnNavigatedTo事件开始");

            switch (e.Parameter)
            {
                // 当从LibraryFolders打开漫画时，传入MangaBook
                case MangaBook mangaBook:
                    await TryChangeMangaBook(mangaBook);
                    break;

                /// 当从文件关联打开时，传入StorageFile
                case StorageFile storageFile:
                    await OnceLoadFromStorageFile(storageFile);
                    break;
            }
        }

        /// <summary>
        /// 直接从StorageFIle里面导入数据，用于第一次实例化时赋值
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        private async Task OnceLoadFromStorageFile(StorageFile storageFile)
        {
            var manga = await ModelFactory.CreateMangaBook(storageFile);

            currentReader = new ReaderVM(manga, storageFile);
            await currentReader.Initial();
            FLIP.ItemsSource = currentReader.BitmapImages;

            var isfilterimage = Configuration.LoadFromFile(App.Current.AppConfigPath)[nameof(General)][nameof(IsFilterImageOn)].BoolValue;
            FilteredImage[] filteredImages = null;
            if (isfilterimage)
            {
                filteredImages = BasicController.DatabaseController.database.FilteredImages.ToArray();
            }
            currentReader.SelectEntries(filteredImages);
            await currentReader.ShowBitmapImages();
        }

        /// <summary>
        /// ReadPage的页面不变，但是更换内部数据源
        /// </summary>
        /// <param name="manga"></param>
        /// <returns></returns>
        private async Task TryChangeMangaBook(MangaBook manga)
        {
            if (manga == null)                          // 传入null，直接跳过
            {
                return;
            }
            if (manga != currentManga)                  // 传入新漫画，则设置新源
            {
                await SetNewSource(manga, manga);
                Debug.WriteLine(this.GetHashCode());
            }
            else                                       // 未传入新漫画，不作变动。这个其实不用写了
            {
                //Do Nothing
            }
        }

        /// <summary>
        /// 切换此页面的书籍源
        /// </summary>
        /// <param name="newmanga"></param>
        /// <param name="manga"></param>
        /// <returns></returns>
        private async Task SetNewSource(MangaBook newmanga, MangaBook manga)
        {
            currentManga = newmanga;
            var oldreader = currentReader;
            currentReader = new ReaderVM(manga);
            oldreader?.Dispose();
            await currentReader.Initial();

            FLIP.ItemsSource = currentReader.BitmapImages;

            var isfilterimage = Configuration.LoadFromFile(App.Current.AppConfigPath)[nameof(General)][nameof(IsFilterImageOn)].BoolValue;
            FilteredImage[] filteredImages = null;
            if (isfilterimage)
            {
                filteredImages = BasicController.DatabaseController.database.FilteredImages.ToArray();
            }
            currentReader.SelectEntries(filteredImages);
            await currentReader.ShowBitmapImages();

            // TODO 任务取消，但是会一直报错
            //await Task.Run(async () => { await currentReader.SelectEntries(filteredImages); });
        }

        #region 数据绑定到已解码的BitmapImage

        private async void FilteThisImage2_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = FLIP.SelectedItem as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            var index = currentReader.BitmapImages.IndexOf(bitmap);
            var entry = currentReader.FilteredArchiveEntries[index];
            currentReader.FilteredArchiveEntries.Remove(entry);
            currentReader.BitmapImages.Remove(bitmap);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await DatabaseController.ImageFilter_Add(hash, length);
            // TODO  这里是存在一个隐患的，可能正在后台多线程使用DbContext

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path, hash + ".jpg");
            entry.WriteToFile(path);
        }

        /// <summary> 此图片另存为 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>

        private async void SaveImageAs2_Click(object sender, RoutedEventArgs e)
        {
            var bitmap = FLIP.SelectedItem as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            var index = currentReader.BitmapImages.IndexOf(bitmap);
            var entry = currentReader.FilteredArchiveEntries[index];
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
        private async void FLIP_SelectionChangedNew(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"SelectionChanged事件开始增加个数：{e.AddedItems.Count}移除个数：{e.RemovedItems.Count}");

            FlipView flipView = sender as FlipView;

            var entry = flipView.SelectedItem as IArchiveEntry;

            if (!(flipView.ContainerFromItem(entry) is FlipViewItem item))
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
        private async void FLIP_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
        private async void FilteThisImage_Click(object sender, RoutedEventArgs e)
        {
            var entry = FLIP.SelectedItem as IArchiveEntry;

            currentReader.FilteredArchiveEntries.Remove(entry);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await DatabaseController.ImageFilter_Add(hash, length);

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path, hash + ".jpg");
            entry.WriteToFile(path);
        }

        [Obsolete]
        private async void SaveImageAs_Click(object sender, RoutedEventArgs e)
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

        private void FLIP_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
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

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            applicationView.TryEnterFullScreenMode();

            ReadPageButtonGroup.Visibility = Visibility.Collapsed;
        }

        private void AppBarToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            applicationView.ExitFullScreenMode();
        }
    }
}