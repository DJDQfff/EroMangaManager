using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyStandard20Library;
using MyUWPLibrary;
using EroMangaTagDatabase.Helper;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views
{
    /// <summary> 功能页，用于存放诸多功能 </summary>
    public sealed partial class FunctionPage : Page
    {
        public FunctionPage()
        {
            this.InitializeComponent();
        }

        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.Background = MyUWPLibrary.WindowsUIColorHelper.GetRandomSolidColorBrush();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var files = await MyUWPLibrary.StorageItemPicker.PickMultiFilesAsync(Windows.Storage.Pickers.PickerLocationId.Downloads, ".zip");
            foreach (var file in files)
            {
                var tags = file.DisplayName.SplitAndParser();
                //TODO：修改本子名的功能，用于去除重复Tag。
                //如：【xxx】【xxx】yyy。zip改名为【xxx】yyy。zip
            }
        }
    }
}