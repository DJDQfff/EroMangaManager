using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using EroMangaManager.Models;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using EroMangaManager.Helpers;
using static System.Net.Mime.MediaTypeNames;
using static EroMangaManager.Helpers.ZipArchiveHelper;
using System.Collections.Generic;
using System.Linq;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class ReadPage : Page
    {
        private Manga currentManga;
        private readonly ObservableCollection<ZipArchiveEntry> zipArchiveEntries = new ObservableCollection<ZipArchiveEntry>();
        private readonly ObservableCollection<BitmapImage> bitmapImages = new ObservableCollection<BitmapImage>();

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
                case Manga manga when currentManga == null:                 // 初次打开，传入一个新漫画
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
                bitmapImages.Clear();
                Stream stream = await currentManga.StorageFile.OpenStreamForReadAsync();
                ZipArchive zipArchive = new ZipArchive(stream);
                zipArchive.GetSortedFilteredEntries(zipArchiveEntries);
                foreach (var entry in zipArchiveEntries)
                {
                    BitmapImage bitmapImage = await OpenEntryAsync(entry);

                    bitmapImages.Add(bitmapImage);
                }
            }
        }

        private async Task SetNewSource0 (Manga manga)
        {
            currentManga = manga;
            bitmapImages.Clear();
            Stream stream = await currentManga.StorageFile.OpenStreamForReadAsync();
            ZipArchive zipArchive = new ZipArchive(stream);
            foreach (var entry in zipArchive.Entries)
            {
                bool cansue = entry.EntryFilter();
                if (cansue)
                {
                    zipArchiveEntries.Add(entry);

                    BitmapImage bitmapImage = await OpenEntryAsync(entry);

                    bitmapImages.Add(bitmapImage);
                }
            }
        }
    }
}