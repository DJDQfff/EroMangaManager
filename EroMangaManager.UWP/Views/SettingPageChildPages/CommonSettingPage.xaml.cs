using EroMangaManager.UWP.SettingEnums;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.SettingPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CommonSettingPage : Page
    {
        /// <summary>
        /// 一般设置页面
        /// </summary>
        public CommonSettingPage()
        {
            this.InitializeComponent();
        }

        private void DirectDeleteOption(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            App.Current.AppConfig.WhetherShowDialogBeforeDelete = toggleSwitch.IsOn;
        }

        private void DeleteOption(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            App.Current.AppConfig.StorageDeleteOption = toggleSwitch.IsOn;
        }

        private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            var b = App.Current.AppConfig.WhetherShowDialogBeforeDelete;

            toggleSwitch.IsOn = b;


        }

        private void ToggleSwitch_Loaded_1(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            var a = App.Current.AppConfig.StorageDeleteOption;
            toggleSwitch.IsOn = a;
        }
    }
}