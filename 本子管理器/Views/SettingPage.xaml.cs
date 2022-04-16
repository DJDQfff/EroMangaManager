using System;

using EroMangaManager.Helpers;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class SettingPage : Page
    {
        public SettingPage ()
        {
            this.InitializeComponent();
        }

        private void Button_Click (object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            string tag = button.Tag as string;
            switch (tag)
            {
                case "SettingFilterImageButton":
                    SettingFrame.Navigate(typeof(SettingSubPages.FiltedImagesPage));

                    break;

                case "SettingTagButton":
                    SettingFrame.Navigate(typeof(SettingSubPages.TagKeywordsManagePage));
                    break;

                case "ErrorZipPageButton":
                    SettingFrame.Navigate(typeof(ErrorZipPage));

                    break;

                default:
                    break;
            }
        }
    }
}