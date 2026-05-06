// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.WinUI3.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FunctionPageShower : Page
    {
        /// <summary>
        /// 所有工具也的集中展示页面
        /// </summary>
        public FunctionPageShower()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var name = button.Name;

            Type type = null;

            switch (name)
            {
                case nameof(Function_FindSameMangaName):
                    type = typeof(FunctionChildPages.FindSameManga);
                    break;

                case nameof(Function_RemoveRepeatTags):
                    type = typeof(FunctionChildPages.RemoveRepeatTags);
                    break;
            }

            //MainPage.Current.MainFrame.Navigate(type, App.Current.GlobalViewModel.ResultMangas);
            FunctionFrame.Navigate(type, App.Current.GlobalViewModel.MangaList);
            return;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            FunctionFrame.GoBack();
        }
    }
}