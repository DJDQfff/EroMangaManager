using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Views.MainPageChildPages;

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace EroMangaManager.UWP.Helpers
{
    internal static class WindowHelper
    {
        internal static async Task ShowNewReadPageWindow(MangaBook mangaBook)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(ReadPage), mangaBook);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();
                Window.Current.Closed += (objectsender, args) =>
                {
                    var page = frame.Content as ReadPage;
                    page.currentReader?.Dispose();
                    GC.SuppressFinalize(Window.Current);
                };
                newViewId = ApplicationView.GetForCurrentView().Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);


        }
    }
}
