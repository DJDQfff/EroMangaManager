using EroMangaManager.Helpers;
using EroMangaManager.Models;
using EroMangaManager.ViewModels;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BookcaseContainer : Page
    {
        internal MangasFolder BindMangaFolder { set; get; }

        /// <summary>
        ///
        /// </summary>
        public BookcaseContainer ()
        {
            this.InitializeComponent();
            App.Current.bookcaseContainer = this;
        }

        internal void ChangeMangasFolder (MangasFolder mangasFolder)
        {
            BindMangaFolder = mangasFolder;

            Bookcase bookcase;
            if (!App.Current.pageInstancesManager.Bookcases.ContainsKey(mangasFolder))
            {
                bookcase = new Bookcase(mangasFolder);
                App.Current.pageInstancesManager.Bookcases.Add(mangasFolder , bookcase);
            }
            else
            {
                bookcase = App.Current.pageInstancesManager.Bookcases[mangasFolder];
            }

            var c = this.FindName("ShowBookcase") as Frame;
            c.Content = bookcase;
        }

        //TODO 因为原来的Bookcase被拆分为Bookcase和BookcaseContainer两个类，所以这个方法现在有bug
        private async void RefreshMangaList (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;

            await CoverHelper.ClearCovers();

            var folder = MyUWPLibrary.AccestListHelper.GetAvailableFutureFolder().Result.ToArray();

            App.Current.collectionObserver.Initialize(folder);

            button.IsEnabled = true;
        }

        //TODO 因为原来的Bookcase被拆分为Bookcase和BookcaseContainer两个类，所以这个方法现在有bug
        private async void TranslateEachMangaName (object sender , Windows.UI.Xaml.RoutedEventArgs e)
        {
            AppBarButton button = sender as AppBarButton;
            button.IsEnabled = false;

            try
            {
                await Translator.TranslateAllMangaName();
            }
            catch
            {
            }
            var bookcase = this.ShowBookcase.Content as Bookcase;
            var gridView = bookcase.FindName("Bookcase_GridView") as GridView;
            var items = gridView.Items;
            foreach (var item in items)
            {
                var manga = item as MangaBook;
                GridViewItem grid = gridView.ContainerFromItem(item) as GridViewItem;
                var root = grid.ContentTemplateRoot as Grid;
                var run = root.FindName("runtext") as Windows.UI.Xaml.Documents.Run;
                run.Text = manga.TranslatedMangaName;
            }

            button.IsEnabled = true;
        }
    }
}