using System;

using EroMangaManager.Views.MainPageChildPages;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace EroMangaManager
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// MainPage的单一实例
        /// </summary>
        internal static MainPage current { set; get; }

        /// <summary>
        ///
        /// </summary>
        public MainPage ()
        {
            this.InitializeComponent();

            current = this;
            MainFrame.Navigate(typeof(BookcaseContainer));

            if (App.Current.collectionObserver.StorageFolders.Count == 0)
            {
                MainFrame.Navigate(typeof(LibraryPage));
            }
        }

        private void MainNavigationView_ItemInvoked (NavigationView sender , NavigationViewItemInvokedEventArgs args)
        {
            Type type = null;

            if (args.IsSettingsInvoked)
                type = typeof(SettingPage);
            else
                switch (args.InvokedItemContainer.Name)
                {
                    case nameof(BookcaseItemContainer):
                        type = typeof(BookcaseContainer);
                        break;

                    case nameof(ListItem):
                        type = typeof(LibraryPage);
                        break;

                    case nameof(ReadItem):
                        type = typeof(ReadPage);
                        break;

                    case nameof(FunctionPageShower):
                        type = typeof(FunctionPageShower);
                        break;
                }

            if (!type.Equals(MainFrame.CurrentSourcePageType))
            {
                MainFrame.Navigate(type);
            }
        }
    }
}