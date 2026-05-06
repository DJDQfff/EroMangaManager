// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板
namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GlobalSearchPage : Page
    {
        private readonly SearchMangaViewModel viewmodel = new();

        /// <summary>
        ///
        /// </summary>
        public GlobalSearchPage()
        {
            InitializeComponent();
            App.Current.GlobalViewModel.EventAfterDeleteMangaSource += x => { viewmodel.Sources.Remove(x); viewmodel.ResultMangas.Remove(x); };

            viewmodel.ResultNewAdd += async x => await App.Current.CoverSetter.AddWork(x);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            viewmodel.Sources = App.Current.GlobalViewModel.MangaList;
            viewmodel.AllTags = await Task.Run(() => viewmodel.Sources.SelectMany(x => x.Tags)
                .Distinct()
                .ToList());

            switch (e.Parameter)
            {
                case SearchParameter searchParameter:
                    {
                        var tags = searchParameter.Tags;
                        foreach (var tag in tags)
                        {
                            MangaTagTokenizingTextBox.AddTokenItem(tag);
                        }
                        //SearchStartButton_Click(SearchStartButton , new RoutedEventArgs());
                    }
                    break;
                //直接把manga传进来，参数自己修改
                case Manga manga:
                    {
                        viewmodel.RequiredText = manga.MangaName;

                        viewmodel.RequiredTags.Clear();
                        foreach (var tag in manga.Tags)
                        {
                            MangaTagTokenizingTextBox.AddTokenItem(tag);
                        }
                    }
                    break;

                case string manganame:
                    {
                        viewmodel.RequiredText = manganame;
                        viewmodel.RequiredTags.Clear();
                    }
                    break;

                case IEnumerable<string> tags:
                    {
                        viewmodel.RequiredText = string.Empty;
                        viewmodel.RequiredTags.Clear();
                        foreach (var tag in tags)
                        {
                            viewmodel.RequiredTags.Add(tag);
                        }
                    }
                    break;
            }
            viewmodel.Search();
        }

        // TODO 搞不清这个干嘛的
        private void TagTokenBox_TokenItemAdding(
            TokenizingTextBox sender,
            TokenItemAddingEventArgs args
        )
        {
            var t = args.TokenText;
            if (!viewmodel.AllTags.Contains(t))
            {
                args.Cancel = true;
            }
        }

        private void TagTokenBox_TokenItemChanged(TokenizingTextBox sender, object args)
        {
            viewmodel.Search();
        }

        private void NameBox_TextChanged(
            AutoSuggestBox sender,
            AutoSuggestBoxTextChangedEventArgs args
        )
        {
            viewmodel.Search();
        }

        //private void ShowInBookcaseButton_Click (object sender , RoutedEventArgs e)
        //{
        //    var result = ResultGridView.ItemsSource;

        //    var condition = result as IEnumerable<Manga>;

        //    var mangasfolder = new MangasGroup()
        //    {
        //        ShowString = (
        //            MangaNameAugoSuggestBox.Text + "+" + MangaTagTokenizingTextBox.SelectedTokenText
        //        ).Trim('+') ,
        //    };
        //    mangasfolder.Mangas.AddRange(condition);
        //    MainPage.Current.MainFrame.Navigate(typeof(Bookcase) , mangasfolder);
        //}

        private void TagTokenizingTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            viewmodel.FiltTags(MangaTagTokenizingTextBox.Text);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as ListBox;
            var index = control.SelectedIndex;
            if (index != -1)
            {
                var key = "datatemplate" + index;
                var resource = this.Resources[key] as DataTemplate;
                ResultGridView.ItemTemplate = resource;
            }
        }
    }
}