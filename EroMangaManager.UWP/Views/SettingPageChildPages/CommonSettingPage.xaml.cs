using EroMangaManager.UWP.SettingEnums;

using SharpConfig;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using static EroMangaManager.UWP.SettingEnums.General;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views.SettingPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CommonSettingPage : Page
    {
        private Configuration Settings { get; set; }

        /// <summary>
        /// 一般设置页面
        /// </summary>
        public CommonSettingPage()
        {
            this.InitializeComponent();
            Settings = Configuration.LoadFromFile(App.Current.AppConfigPath);
        }

        private void DirectDeleteOption(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            App.Current.AppConfig[nameof(General)][nameof(WhetherShowDialogBeforeDelete)].BoolValue = toggleSwitch.IsOn;
            App.Current.AppConfig.SaveToFile(App.Current.AppConfigPath);
        }

        private void DeleteOption(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            App.Current.AppConfig[nameof(General)][nameof(StorageDeleteOption)].BoolValue = toggleSwitch.IsOn;
            App.Current.AppConfig.SaveToFile(App.Current.AppConfigPath);
        }

        private void ToggleSwitch_Loaded(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;

            toggleSwitch.IsOn = App.Current.AppConfig[nameof(General)][nameof(WhetherShowDialogBeforeDelete)].BoolValue;
        }

        private void ToggleSwitch_Loaded_1(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            toggleSwitch.IsOn = App.Current.AppConfig[nameof(General)][nameof(StorageDeleteOption)].BoolValue;
        }
    }
}