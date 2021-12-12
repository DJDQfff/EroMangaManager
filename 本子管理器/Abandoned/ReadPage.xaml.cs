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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace Abandoned.EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class ReadPage : Page
    {
        private Manga eroManga;
        private ObservableCollection<ZipArchiveEntry> zipArchiveEntries = new ObservableCollection<ZipArchiveEntry>();
        private ObservableCollection<BitmapImage> bitmapImages = new ObservableCollection<BitmapImage>();
        private int index = 0;

        public ReadPage ()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //判断是否来自Bookcase页面跳转,这很重要
            if (e.Parameter is Manga)
            {
                eroManga = e.Parameter as Manga;
                Stream stream = await eroManga.StorageFile.OpenStreamForReadAsync();
                ZipArchive zipArchive = new ZipArchive(stream);
                foreach (var entry in zipArchive.Entries)
                {
                    if (entry.Length != 0)
                    {
                        zipArchiveEntries.Add(entry);

                        //BitmapImage bitmapImage = await OpenEntryAsync(entry);

                        //bitmapImages.Add(bitmapImage);
                    }
                }
            }
            BitmapImage bitmapImage = await OpenEntryAsync(zipArchiveEntries[1]);
            Image.Source = bitmapImage;
            ProgressRing.Visibility = Visibility.Collapsed;
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