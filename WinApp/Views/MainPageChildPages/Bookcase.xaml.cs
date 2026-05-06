// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

using System.ComponentModel;

namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Bookcase : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string index = null;

        private ObservableCollection<Manga> FilteredMangas = [];

        public string GridViewItemTemplateIndex
        {
            get => index;
            set
            {
                if (value != null)
                {
                    index = value;
                    var key = "BookcaseTemplate" + index;
                    Bookcase_GridView.ItemTemplate = Resources[key] as DataTemplate;
                    //_ = new DataTemplate();//不知道这个干嘛的
                    App.Current.AppConfig.AppConfig.General.BookcaseTemplateKey = index;
                }
            }
        }

        private MangasGroup mangasGroup;

        public MangasGroup MangasGroup
        {
            get => mangasGroup;
            set
            {
                mangasGroup = value;

                Bookcase_GridView.ItemsSource = value.Mangas;
                Bookcase_HintTextBlock.Visibility = Visibility.Collapsed;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MangasGroup)));
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Bookcase()
        {
            InitializeComponent();
            GridViewItemTemplateIndex = "1";
            //MangasGroup = App.Current.GlobalViewModel.MangaFolders.FirstOrDefault();
        }

        /// <summary>
        /// 导航时，传入要绑定的数据
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            MainPage.Current.MainNavigationView.SelectedItem = MainPage
                .Current
                .MainNavigationView
                .MenuItems[0];

            switch (e.Parameter)
            {
                case MangasGroup mangasFolder:
                    if (mangasFolder != MangasGroup)
                        MangasGroup = mangasFolder;
                    await App.Current.CoverSetter.AddWork(mangasFolder.Mangas);
                    break;
            }
        }

        //TODO 本子名翻译功能。 因为原来的Bookcase被拆分为Bookcase和Bookcase两个类，所以这个方法现在有bug
        private async void TranslateEachMangaName(object sender, RoutedEventArgs e)
        {
            var button = sender as AppBarButton;
            button.IsEnabled = false;

            try
            {
                await Translator.TranslateAllMangaName();
            }
            catch { }

            var items = Bookcase_GridView.Items;
            foreach (var item in items)
            {
                var manga = item as Manga;
                var grid = Bookcase_GridView.ContainerFromItem(item) as GridViewItem;
                var root = grid.ContentTemplateRoot as Grid;
                var run = root.FindName("runtext") as Microsoft.UI.Xaml.Documents.Run;
                run.Text = manga.TranslatedMangaName;
            }

            button.IsEnabled = true;
        }

        private void Order(object sender, RoutedEventArgs e)
        {
            MangasGroup?.SortMangas(x => x.FileSize);
        }

        private async void Combochangefolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MangasGroup = e.AddedItems[0] as MangasGroup;
            await App.Current.CoverSetter.AddWork(MangasGroup.Mangas);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuFlyoutItem;
            var index = item.Tag as string;
            GridViewItemTemplateIndex = index;
        }

    }
}