using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

using EroMangaManager.Helpers;
using EroMangaManager.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using static EroMangaManager.Helpers.ZipEntryHelper;
using System;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class ReadPage : Page
    {
        private Manga currentManga;
        private readonly ObservableCollection<ZipArchiveEntry> zipArchiveEntries = new ObservableCollection<ZipArchiveEntry>();
        public Reader currentReader;

        public ReadPage ()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MainPage.current.MainNavigationView.IsPaneOpen = false;

            // 判断数据类型,这很重要
            switch (e.Parameter)
            {
                case Manga manga when currentManga == null:                 // 未打开漫画，传入一个新漫画
                    await SetNewSource(manga);
                    break;

                case Manga manga when currentManga == manga:                // 传入的漫画和已打开漫画是同一本
                    //Do Nothing
                    break;

                case Manga manga when currentManga != manga:                // 传入的新漫画和已打开漫画不一致
                    await SetNewSource(manga);
                    break;
            }
            async Task SetNewSource (Manga manga)
            {
                currentManga = manga;
                zipArchiveEntries.Clear();
                currentReader = await Reader.Create(manga);
                Debug.WriteLine(currentReader.GetHashCode());
                currentReader.OpenEntries(zipArchiveEntries);
            }
        }

        private async void FLIP_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            int c = e.AddedItems.Count;

            if (c == 0)                 // 从集合中移出项，触发两次此事件，第一次additems个数为0，第二次additems不为0
                return;

            ZipArchiveEntry entry = e.AddedItems[0] as ZipArchiveEntry;

            FlipViewItem item = FLIP.ContainerFromItem(entry) as FlipViewItem;      // TODO 一个稳定必现的bug，创建封面文件后，打开本子，这里会提示item为null
            var root = item.ContentTemplateRoot as Grid;

            //root.FindName("image");               // TODO 此方法有bug，应该是控件bug，有空翻文档细看

            var sc = root.Children[0] as ScrollViewer;
            var image = sc.Content as Image;
            image.Source = await ShowEntryAsync(entry);
        }

        /// <summary>
        /// 移出项时，执行一次此方法，然后引发两次SelectionChanged事件
        /// </summary>
        /// <param name="sender"> </param>
        /// <param name="e">      </param>
        private async void MenuFlyoutItem_Click (object sender, RoutedEventArgs e)
        {
            ZipArchiveEntry entry = FLIP.SelectedItem as ZipArchiveEntry;
            zipArchiveEntries.Remove(entry);

            string hash = entry.ComputeHash();
            HashManager.Add(hash);

            StorageFolder storageFolder = await FoldersHelper.GetFilterFolder();
            var storageFiles = await storageFolder.GetFilesAsync();
            int count = storageFiles.Count;
            string path = Path.Combine(storageFolder.Path, count + 1 + ".jpg");
            entry.ExtractToFile(path);
        }

        private async void MenuFlyoutItem_Click_1 (object sender, RoutedEventArgs e)
        {
            ZipArchiveEntry entry = FLIP.SelectedItem as ZipArchiveEntry;
            StorageFile storageFile = await PickHelper.SavePicture();
            if (storageFile != null)
            {
                Stream stream = await storageFile.OpenStreamForWriteAsync();
                Stream stream1 = entry.Open();
                await stream1.CopyToAsync(stream);
            }
        }
    }
}