using System.Collections.Generic;
using System.Linq;

using EroMangaManager.Core.Models;
using EroMangaManager.Core.ViewModels;
using MyLibrary.Standard20;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchMangaPage : Page
    {
        private TagManagerViewModel searchMangaViewModel;

        /// <summary>
        ///
        /// </summary>
        public SearchMangaPage()
        {
            this.InitializeComponent();
            var mangas = App.Current.GlobalViewModel.MangaList;
            searchMangaViewModel = new TagManagerViewModel(mangas.Select(x => x.MangaTagsIncludedInFileName));
        }

        private void TokenizingTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var a = sender.Text;
            tokenbox.SuggestedItemsSource = searchMangaViewModel.Search(a);
        }

        private void tokenbox_TokenItemAdding(Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox sender, Microsoft.Toolkit.Uwp.UI.Controls.TokenItemAddingEventArgs args)
        {
            var t = args.TokenText;
            if (!searchMangaViewModel.AllTags.Contains(t))
            {
                args.Cancel = true;
            }
        }

        private void tokenbox_TokenItemAdded(Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox sender, object args)
        {
            var token = args as string;
            searchMangaViewModel.SelectedTags.Add(token);
            searchMangaViewModel.HideTag(token);
        }

        private void tokenbox_TokenItemRemoved(Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox sender, object args)
        {
            var token = args as string;

            searchMangaViewModel.SelectedTags.Remove(token);
            searchMangaViewModel.CancelHideTag(token);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var text = sender.Text; if (text != null)
            {
                var a = App.Current.GlobalViewModel.MangaList.Where(x => x.MangaName.Contains(text)).Select(x => x.MangaName).ToList();
                sender.ItemsSource = a;
            }
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var manganame = NameSearchInput.Text;

            var tags = new List<string>(searchMangaViewModel.SelectedTags)
            {
                manganame
            };

            var requiredMatchCount = tags.Count;

            var allmangas = App.Current.GlobalViewModel.MangaList;

            var conditions = allmangas
                .Where(x => tags
                    .Count(y => x.FileDisplayName.Contains(y)) == requiredMatchCount);

            ResultGridView.ItemsSource = conditions;
        }

        private void Button_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var result=ResultGridView.ItemsSource;

            var condition = result as IEnumerable<MangaBook>;

            var mangasfolder = new MangasFolder(null);
            mangasfolder.MangaBooks.AddRange(condition);
            MainPage.Current.MainFrame.Navigate(typeof(Bookcase), mangasfolder);


        }
    }
}