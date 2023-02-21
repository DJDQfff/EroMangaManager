using System.Linq;
using System;
using EroMangaManager.ViewModels;


using Windows.UI.Xaml.Controls;
using System.Collections.Generic;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchMangaPage : Page
    {
        private TagManagerViewModel searchMangaViewModel;

        public SearchMangaPage ()
        {
            this.InitializeComponent();
            var mangas = App.Current.collectionObserver.MangaList;
            searchMangaViewModel = new TagManagerViewModel(mangas.Select(x => x.MangaTags));
        }

        private void TokenizingTextBox_TextChanged (AutoSuggestBox sender , AutoSuggestBoxTextChangedEventArgs args)
        {
            var a = sender.Text;
            tokenbox.SuggestedItemsSource = searchMangaViewModel.Search(a);

        }

        private void tokenbox_QuerySubmitted (AutoSuggestBox sender , AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var a = tokenbox.Items;
            var list = new List<string>();
            foreach(var item in a)
            {
                var b = item as string;
                list.Add(b);
            }



        }
    }
}