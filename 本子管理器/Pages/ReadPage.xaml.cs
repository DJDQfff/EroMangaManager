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

using static EroMangaManager.Helpers.ZipArchiveHelper;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class ReadPage : Page
    {
        private Manga currentManga;
        private readonly ObservableCollection<ZipArchiveEntry> zipArchiveEntries = new ObservableCollection<ZipArchiveEntry>();
        private ObservableCollection<BitmapImage> bitmapImages = new ObservableCollection<BitmapImage>();
        public Reader currentReader;
        private Reader pastReader;

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
                bitmapImages.Clear();
                currentReader = await Reader.Create(manga);
                Debug.WriteLine(currentReader.GetHashCode());
                currentReader.OpenEntries(zipArchiveEntries);
            }
        }

        private async void Image_Loaded (object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            ZipArchiveEntry zipArchiveEntry = FLIP.SelectedItem as ZipArchiveEntry;
            // TODO ZipArchiveEntry entry =
            // FLIP.SelectedItem;
            image.Source = await OpenEntryAsync(zipArchiveEntry);
        }

        private async void FLIP_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            ZipArchiveEntry entry = e.AddedItems[0] as ZipArchiveEntry;
            FlipViewItem item = FLIP.ContainerFromItem(entry) as FlipViewItem;
            var root = item.ContentTemplateRoot as Grid;

            //root.FindName("image");               // TODO 此方法有bug，应该是控件bug，有空翻文档

            var sc = root.Children[0] as ScrollViewer;
            var image = sc.Content as Image;
            image.Source = await OpenEntryAsync(entry);
        }
    }
}