using System;
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

using iText.Layout.Font;

using SharpCompress.Archives;

using SharpConfig;

using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
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

        private StorageFile currentStorageFile;

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
            switch (e.Parameter)
            {
                // 当从LibraryFolders打开漫画时，传入MangaBook
                case MangaBook manga:
                    if (manga != currentManga)                  // 传入新漫画，则设置新源
                    {
                        currentManga = manga;
                        var oldreader = currentReader;
                        currentStorageFile = await MyLibrary.UWP.AccestListHelper.GetStorageFile(manga.FilePath);
                        currentReader = new ReaderVM(manga, currentStorageFile);
                        oldreader?.Dispose();
                    }
                    break;

                // 当从文件关联打开时，传入StorageFile
                case StorageFile storageFile:
                    {
                        currentStorageFile = storageFile;
                        currentManga = await ModelFactory.CreateMangaBook(currentStorageFile);

                        currentReader = new ReaderVM(currentManga, currentStorageFile);
                    }
                    break;
            }

            await currentReader.Initial();
            FLIP.ItemsSource = currentReader.FilteredArchiveImageEntries;
            var isfilterimage = Configuration.LoadFromFile(App.Current.AppConfigPath)[nameof(General)][nameof(IsFilterImageOn)].BoolValue;
            FilteredImage[] filteredImages = null;
            if (isfilterimage)
            {
                filteredImages = BasicController.DatabaseController.database.FilteredImages.ToArray();
            }
            currentReader.SelectEntries(filteredImages);
        }

        /// <summary>
        /// 这个不知道为什么就能正常工作，不会触发bug
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void FLIP_SelectionChangedNew(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine($"SelectionChanged事件开始");

            FlipView flipView = sender as FlipView;
            var entry = flipView.SelectedItem as IArchiveEntry;

            if (flipView.ContainerFromItem(entry) is FlipViewItem item)
            {
                var root = item.ContentTemplateRoot as Grid;

                var image = root.FindName("image") as Image;

                image.Source = await entry.ShowEntryAsync();
                Debug.WriteLine($"SelectionChanged事件结束");
            }
        }

        /// <summary> 添加此图片到过滤图库 </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>

        private async void FilteThisImageByEntry(object sender, RoutedEventArgs e)
        {
            var entry = FLIP.SelectedItem as IArchiveEntry;

            currentReader.FilteredArchiveImageEntries.Remove(entry);
            string hash = entry.ComputeHash();
            long length = entry.Size;
            await DatabaseController.ImageFilter_Add(hash, length);

            StorageFolder storageFolder = await GetChildTemporaryFolder(nameof(Filters));
            string path = Path.Combine(storageFolder.Path, hash + ".jpg");
            entry.WriteToFile(path);
        }

        private async void SaveImageAs_Click(object e, RoutedEventArgs args)
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

        private void FLIP_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var a = ControlsGrid.Visibility;
            if (a == Visibility.Collapsed)
            {
                ControlsGrid.Visibility = Visibility.Visible;
            }
            else
            {
                ControlsGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            applicationView.TryEnterFullScreenMode();

            ControlsGrid.Visibility = Visibility.Collapsed;
        }

        private void AppBarToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            applicationView.ExitFullScreenMode();
        }

        private void ReadPositionSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var entryindex = Convert.ToInt32(ReadPositionSlider.Value - 1);
            FLIP.SelectedIndex = entryindex;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            currentReader?.Dispose();
        }
    }
}