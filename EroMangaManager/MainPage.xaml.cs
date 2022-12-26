using System;

using EroMangaManager.Models;
using EroMangaManager.Views;
using EroMangaManager.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.ApplicationModel.Resources;
using Org.BouncyCastle.Asn1.X509.Qualified;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804
// 上介绍了“空白页”项模板

namespace EroMangaManager
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class MainPage : Page
    {
        internal BookcaseContainer bookcaseContainer;

        /// <summary>
        /// CollectionObserver实例
        /// </summary>
        internal ObservableCollectionVM collectionObserver { get; }

        internal static PageInstancesManager pageInstancesManager = new PageInstancesManager();

        /// <summary>
        /// MainPage的单一实例
        /// </summary>
        internal static MainPage current { set; get; }

        /// <summary>
        /// 系统resw解析实例
        /// </summary>
        public ResourceLoader stringsresourceLoader { get; } = ResourceLoader.GetForCurrentView("StringResources");

        /// <summary>
        ///
        /// </summary>
        public MainPage ()
        {
            this.InitializeComponent();

            var folder = MyUWPLibrary.AccestListHelper.GetAvailableFutureFolder().Result.ToArray();
            collectionObserver = new ObservableCollectionVM(folder);

            collectionObserver.ErrorZipEvent += (string str) =>
              {
                  new ToastContentBuilder()
                    .AddText($"{str}\r{stringsresourceLoader.GetString("ErrorString1")}")
                    .Show();
              };

            collectionObserver.WorkDoneEvent += (string str) =>
            {
                new ToastContentBuilder()
  .AddText(str)
  .Show();
            };
            current = this;
            pageInstancesManager.MainPage = this;
            MainFrame.Navigate(typeof(BookcaseContainer));

           if(collectionObserver.StorageFolders.Count == 0 )
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
                    case nameof(FunctionItem):
                        type = typeof(FunctionPage);
                        break;

                }

            if (!type.Equals(MainFrame.CurrentSourcePageType))
            {
                MainFrame.Navigate(type);
            }
        }
    }
}