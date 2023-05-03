using System;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.Views;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EroMangaManager.UWP.Helpers
{
    internal static class WindowHelper
    {
        internal static async Task ShowNewWindow(Type _type, object data)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var rootFrame = new Frame();
                rootFrame.Navigate(_type, data);

                switch (data)       // 设置窗口标题文字
                {
                    case MangaBook manga:
                        ApplicationView.GetForCurrentView().Title = manga.MangaName;
                        break;

                    case RepeatMangaBookGroup group:
                        ApplicationView.GetForCurrentView().Title = group.Key;
                        break;
                }

                Window.Current.Content = rootFrame;
                Window.Current.Closed += (objectsender, args) =>
                {
                    var page = rootFrame.Content as ReadPage;
                    page.currentReader?.Dispose();
                    GC.SuppressFinalize(Window.Current);
                };

                // You have to activate the window in order to show it later.
                Window.Current.Activate();

                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }
    }
}