using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.MainPageChildPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class SettingPage : Page
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public SettingPage ()
        {
            this.InitializeComponent();
        }

        private void NavigationView_ItemInvoked (NavigationView sender , NavigationViewItemInvokedEventArgs args)
        {
            switch (args.InvokedItemContainer.Name)
            {
                case nameof(CommonSettingNavigationViewItem):
                    SettingFrame.Navigate(typeof(SettingPageChildPages.CommonSettingPage));
                    break;

                case nameof(SettingFilterImageButton):
                    SettingFrame.Navigate(typeof(SettingPageChildPages.FiltedImagesPage));
                    break;

                case nameof(SettingTagButton):
                    SettingFrame.Navigate(typeof(SettingPageChildPages.TagKeywordsManagePage));
                    break;

                case nameof(ErrorZipPageButton):
                    SettingFrame.Navigate(typeof(SettingPageChildPages.ErrorZipPage));
                    break;
            }
        }

        private void SettingFrame_Loaded (object sender , RoutedEventArgs e)
        {
            SettingFrame.Navigate(typeof(SettingPageChildPages.AboutPage));
        }
    }
}