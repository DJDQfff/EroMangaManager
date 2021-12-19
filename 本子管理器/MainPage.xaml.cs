using System;

using EroMangaManager.Models;
using EroMangaManager.Pages;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace EroMangaManager
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class MainPage : Page
    {
        public ListObserver listObserver = new ListObserver();

        public static MainPage current;

        public MainPage ()
        {
            this.InitializeComponent();
            current = this;
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            MainFrame.Navigate(typeof(Bookcase));
        }

        private void MainNavigationView_ItemInvoked (NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Type type = null;
            if (args.IsSettingsInvoked)
                type = typeof(SettingPage);
            switch (args.InvokedItemContainer.Tag)
            {
                case "bookcase":
                    type = typeof(Bookcase);
                    break;

                case "list":
                    type = typeof(LibraryPage);
                    break;

                case "read":
                    type = typeof(ReadPage);
                    break;
            }
            if (!type.Equals(MainFrame.CurrentSourcePageType))
            {
                MainFrame.Navigate(type);
            }
        }
    }
}