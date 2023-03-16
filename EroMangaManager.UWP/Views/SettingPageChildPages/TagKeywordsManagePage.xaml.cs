using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.SettingPageChildPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class TagKeywordsManagePage : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public TagKeywordsManagePage()
        {
            this.InitializeComponent();
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            int index = comboBox.SelectedIndex;
        }
    }
}