using System;
using System.Linq;

using EroMangaManager.UWP.SettingEnums;
using EroMangaManager.UWP.Views.MainPageChildPages;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace EroMangaManager.UWP
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// MainPage的单一实例
        /// </summary>
        internal static MainPage Current { set; get; }

        /// <summary>
        ///
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();

            Current = this;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            var defaultfolderpath = ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.DefaultBookcaseFolder.ToString()] as string;
            var defaultfolder = App.Current.GlobalViewModel.MangaFolders.SingleOrDefault(x => x.FolderPath == defaultfolderpath);

                MainFrame.Navigate(typeof(Bookcase), defaultfolder);
        }

        private void MainNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            Type type = null;

            if (args.IsSettingsInvoked)
                type = typeof(SettingPage);
            else
                switch (args.InvokedItemContainer.Name)
                {
                    case nameof(BookcaseItem):
                        type = typeof(Bookcase);
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

                    case nameof(TagsManagePage):
                        type = typeof(TagsManagePage);
                        break;

                    case nameof(SearchMangaPage):
                        type = typeof(SearchMangaPage);
                        break;
                }

            if (!type.Equals(MainFrame.CurrentSourcePageType))
            {
                MainFrame.Navigate(type);
            }
        }

        private void UpdateRecordItem_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(Views.SettingPageChildPages.UpdateRecordsPage));
        }

        private void UsageButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(Views.SettingPageChildPages.UsageDocumentPage));
        }
    }
}