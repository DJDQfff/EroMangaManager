using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using EroMangaManager.Models;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.SettingPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CommonSettingPage : Page
    {
        /// <summary>
        /// 一般设置页面
        /// </summary>
        public CommonSettingPage ()
        {
            this.InitializeComponent();
        }

        private void DirectDeleteOption (object sender , RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.WhetherShowDialogBeforeDelete.ToString()] = toggleSwitch.IsOn;

        }

        private void DeleteOption (object sender , RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.StorageDeleteOption.ToString()] = toggleSwitch.IsOn;

        }

        private void ToggleSwitch_Loaded (object sender , RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            var a = (bool) (ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.WhetherShowDialogBeforeDelete.ToString()] ?? false);
        toggleSwitch.IsOn= a;
        }

        private void ToggleSwitch_Loaded_1 (object sender , RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            var a = (bool) (ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.StorageDeleteOption.ToString()] ?? false);
            toggleSwitch.IsOn = a;

        }
    }
}
