using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using EroMangaManager.Helpers;
using EroMangaManager.Models;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static EroMangaManager.Models.FolderEnum;
using EroMangaManager.ViewModels;
using static EroMangaManager.Helpers.ZipEntryHelper;
using static MyUWPLibrary.StorageItemPicker;
using static MyUWPLibrary.StorageFolderHelper;
using static MyStandard20Library.HashComputer;
using System.Reflection.PortableExecutable;

using SharpCompress.Archives;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views
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
            MainPage.pageInstancesManager.ReadPage = this;
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
                this.ReadPage_ProgressRing_0.Visibility = Visibility.Visible;
                await SetNewSource(manga);
            }
            else                                       // 未传入新漫画，不作变动。这个其实不用写了
            {
                //Do Nothing
            }

            async Task SetNewSource (MangaBook newmanga)
            {
                currentManga = newmanga;
                currentReader?.Dispose();
                currentReader = await ReaderVM.Creat(newmanga, null);

                FLIP.ItemsSource = currentReader.bitmapImages;
                await currentReader.SelectEntriesAsync();
            }
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
            Debug.WriteLine("睡眠开始");
        }

        private async void FLIP_SelectionChangedNew (object sender, SelectionChangedEventArgs e)
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

        private async void FilteThisImage2_Click (object sender, RoutedEventArgs e)
        {
            var bitmap = FLIP.SelectedItem as Windows.UI.Xaml.Media.Imaging.BitmapImage;
            var index = currentReader.bitmapImages.IndexOf(bitmap);
            var entry = currentReader.zipArchiveEntries[index];
            currentReader.zipArchiveEntries.Remove(entry);
            currentReader.bitmapImages.Remove(bitmap);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await HashManager.Add(hash, length);

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path, hash + ".jpg");
            entry.WriteToFile(path);
        }

        /// <summary> 此图片另存为 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>

        private async void SaveImageAs2_Click (object sender, RoutedEventArgs e)
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

        #region 不用了

        /// <summary> 切换图,这个不在使用 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private async void FLIP_SelectionChanged (object sender, SelectionChangedEventArgs e)
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
        private async void FilteThisImage_Click (object sender, RoutedEventArgs e)
        {
            var entry = FLIP.SelectedItem as IArchiveEntry;

            currentReader.zipArchiveEntries.Remove(entry);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await HashManager.Add(hash, length);

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path, hash + ".jpg");
            entry.WriteToFile(path);
        }

        private async void SaveImageAs_Click (object sender, RoutedEventArgs e)
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

        #endregion 不用了
    }
}