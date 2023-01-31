using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private void Button_Click (object sender , RoutedEventArgs e)
        {
            var button = sender as Button;
            var name = button.Name;

            Type type = null;

            switch (name)
            {
                case nameof(Function_FindSameMangaName):
                    type = typeof(FunctionChildPages.FindSameManga);
                    break;
                    //case nameof(Function_RemoveRepeatTags):
                    //    type = typeof(FunctionChildPages.RemoveRepeatTags);
                    //    break;
            }

            MainPage.Current.MainFrame.Navigate(type , App.Current.collectionObserver.MangaList);

            return;

            // 下面的部分原本是设计来弹出个新窗口的，改为页面跳转
            //CoreApplicationView newView = CoreApplication.CreateNewView();
            //int newViewId = 0;
            //await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal , () =>
            //{
            //    Frame frame = new Frame();
            //    frame.Navigate(type , App.Current.collectionObserver.MangaList);
            //    Window.Current.Content = frame;
            //    // You have to activate the window in order to show it later.
            //    Window.Current.Activate();

            //    newViewId = ApplicationView.GetForCurrentView().Id;
            //});
            //bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
    }
}