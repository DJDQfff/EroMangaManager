using System.Collections.Generic;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Helpers;

using MyLibrary.Standard20;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.FunctionChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RemoveRepeatTags : Page
    {
        /// <summary>
        ///
        /// </summary>
        public RemoveRepeatTags()
        {
            this.InitializeComponent();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var containrepeat = new List<MangaBook>();
            foreach (var book in App.Current.GlobalViewModel.MangaList)
            {
                if (book.MangaTagsIncludedInFileName.ContainRepeat())
                {
                    containrepeat.Add(book);
                }
            }
            list.ItemsSource = containrepeat;
        }

        private async void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var button = sender as Button;
            var manga = button.DataContext as MangaBook;
            string suggestenname = EroMangaDB.Helper.TagBasedStringHelper.RemoveRepeatTag(manga.FileDisplayName);
            await StorageHelper.RenameSourceFile(manga);
        }
    }
}