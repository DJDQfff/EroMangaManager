using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using EroMangaManager.Models;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.FunctionChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FindSameManga : Page
    {
        private ObservableCollection<MangaBookRepeatGroup> repeat = new ObservableCollection<MangaBookRepeatGroup>();

        private List<MangaBook> mangaBooks;

        /// <summary>
        ///
        /// </summary>
        public FindSameManga ()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            mangaBooks = e.Parameter as List<MangaBook>;

            var groups1 = mangaBooks.GroupBy(n => n.MangaName);

            foreach (var group in groups1)
            {
                if (group.Count() > 1)
                {
                    var repeatgroup = new MangaBookRepeatGroup(group);
                    repeat.Add(repeatgroup);
                }
            }
        }

        private void StackPanel_Loaded (object sender , RoutedEventArgs e)
        {
            var a = sender as StackPanel;
            a.Background = MyUWPLibrary.WindowsUIColorHelper.GetRandomSolidColorBrush();
        }

        private async void Button_Click (object sender , RoutedEventArgs e)
        {
            var button = sender as Button;

            MangaBook eroManga = button.DataContext as MangaBook;

            await Helpers.StorageHelper.DeleteSourceFile(eroManga);

        }
    }
}