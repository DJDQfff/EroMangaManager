using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.Views.MainPageChildPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class FunctionPageShower : Page
    {
        /// <summary>
        /// 所有工具也的集中展示页面
        /// </summary>
        public FunctionPageShower ()
        {
            this.InitializeComponent();
        }

        private void Button_Loaded (object sender , RoutedEventArgs e)
        {
            var button = sender as Button;
            button.Background = MyUWPLibrary.WindowsUIColorHelper.GetRandomSolidColorBrush();
        }

        private async void Button_Click (object sender , RoutedEventArgs e)
        {
            var button = sender as Button;
            var xuid = button.Name;

            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal , () =>
            {
                Frame frame = new Frame();

                frame.Navigate(typeof(FunctionChildPages.RemoveTags) , null);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void VariableSizedWrapGrid_Loaded (object sender , RoutedEventArgs e)
        {

        }
    }
}
