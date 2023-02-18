using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using EroMangaManager.ViewModels;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SearchMangaPage : Page
    {
        TagManagerViewModel searchMangaViewModel;
        public SearchMangaPage ()
        {
            this.InitializeComponent();
            var mangas = App.Current.collectionObserver.MangaList;
            searchMangaViewModel = new TagManagerViewModel(mangas.Select(x => x.MangaTags));
        }

        private void autoSuggestBox_TextChanged (AutoSuggestBox sender , AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason is AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var text = sender.Text;
               var a=  searchMangaViewModel.Search(text);
                sender.ItemsSource= a;
            }
        }

        private void autoSuggestBox_QuerySubmitted (AutoSuggestBox sender , AutoSuggestBoxQuerySubmittedEventArgs args)
        {

        }

        private void VariableSizedWrapGrid_Loaded (object sender , RoutedEventArgs e)
        {
            foreach (var x in searchMangaViewModel.AllTags)
            {
                TextBlock textBlock = new TextBlock()
                {
                    Text = x
                };


                var control = sender as VariableSizedWrapGrid;
                 control.Children.Add(textBlock);
            }
        }

    }
}
