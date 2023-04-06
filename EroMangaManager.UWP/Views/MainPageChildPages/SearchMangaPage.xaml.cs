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
        }


        private void tokenbox_TokenItemAdding(Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox sender, Microsoft.Toolkit.Uwp.UI.Controls.TokenItemAddingEventArgs args)
        {
            var t = args.TokenText;
            if (!searchMangaViewModel.AllTags.Contains(t))
            {
                args.Cancel= true;
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


        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var list = searchMangaViewModel.SelectedTags;
            var requiredCount = list.Count;
            var allmangas = App.Current.GlobalViewModel.MangaList;
            var conditionMangas = allmangas.Where(x => x.MangaTagsIncludedInFileName.Count(y => list.Contains(y)) == requiredCount);

            ResultGridView.ItemsSource = conditionMangas;

        }
    }
}