using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;

using EroMangaManager.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

using static System.Net.Mime.MediaTypeNames;
using static EroMangaManager.Helpers.ZipArchiveHelper;
using EroMangaManager.Helpers;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class ReadPage : Page
    {
        private Manga eroManga;
        private ObservableCollection<ZipArchiveEntry> zipArchiveEntries;
        private ObservableCollection<BitmapImage> bitmapImages = new ObservableCollection<BitmapImage>();
        private int index = 0;
        private List<BitmapImage> images = new List<BitmapImage>(5);

        public ReadPage ()
        {
            this.InitializeComponent();
        }

        // TODO:添加ocr功能，实现实时翻译，实例代码可以参考
        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // 判断数据类型,这很重要
            switch (e.Parameter)
            {
                case Manga manga when eroManga == null:
                    SetSource(manga);
                    break;

                case Manga manga when eroManga == manga:
                    //Do Nothing
                    break;

                case Manga manga when eroManga != manga:
                    SetSource(manga);
                    break;
            }
            MainPage.current.MainNavigationView.IsPaneOpen = false;
            ProgressRing.Visibility = Visibility.Collapsed;

            async void SetSource (Manga tempmanga)
            {
                eroManga = tempmanga;
                List<ZipArchiveEntry> list = await ZipArchiveHelper.GetEntriesAsync(tempmanga.StorageFile);
                zipArchiveEntries = new ObservableCollection<ZipArchiveEntry>(list);
                BitmapImage bitmapImage = await OpenEntryAsync(zipArchiveEntries[0]);
                Image.Source = bitmapImage;
            }
        }

        protected override void OnNavigatedFrom (NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            MainPage.current.MainNavigationView.IsPaneOpen = true;
        }

        private async void LastImage_Click (object sender, RoutedEventArgs e)
        {
            if (index != 0)
            {
                index -= 1;
                BitmapImage bitmapImage = await OpenEntryAsync(zipArchiveEntries[index]);
                Image.Source = bitmapImage;
            }
        }

        private async void NextImage_Click (object sender, RoutedEventArgs e)
        {
            if (index != zipArchiveEntries.Count - 1)
            {
                index += 1;
                BitmapImage bitmapImage = await OpenEntryAsync(zipArchiveEntries[index]);
                Image.Source = bitmapImage;
            }
        }
    }
}