using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml.Controls;
using EroMangaManager.Core.ViewModels;
using EroMangaManager.Core.Models;

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
            searchMangaViewModel.Remove(a);
        }

        private void tokenbox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var tokens = tokenbox.Items;
            var list = new List<string>();
            foreach (var item in tokens)
            {
                var b = item as string;  // TODO 试试协变逆变
               if(b!=null) list.Add(b);
            }
            var requiredCount = list.Count;
            var allmangas = App.Current.GlobalViewModel.MangaList;
            var conditionMangas = allmangas.Where(x => x.MangaTagsIncludedInFileName.Count(y => list.Contains(y)) == requiredCount);

            ResultGridView.ItemsSource= conditionMangas;
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var text=sender.Text; if(text!=null)
            {
                var a=App.Current.GlobalViewModel.MangaList.Where(x=>x.MangaName.Contains(text)).Select(x=>x.MangaName).ToList();
                sender.ItemsSource = a;
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item=args.SelectedItem as string;

            var a = App.Current.GlobalViewModel.MangaList.Where(x => x.MangaName.Contains(item)).ToList();
            ResultGridView.ItemsSource = a;

        }
    }
}