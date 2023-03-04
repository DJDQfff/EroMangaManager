﻿using System;
using System.Threading.Tasks;

using EroMangaManager.Helpers;
using EroMangaManager.Models;
using EroMangaManager.ViewModels;
using EroMangaManager.Views.MainPageChildPages;

using Microsoft.Toolkit.Uwp.Notifications;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static EroMangaDB.BasicController;
using static EroMangaManager.Models.FolderEnum;
using static MyUWPLibrary.StorageFolderHelper;

namespace EroMangaManager
{
    /// <summary> 提供特定于应用程序的行为，以补充默认的应用程序类。 </summary>
    public sealed partial class App : Application
    {
        internal Bookcase bookcaseContainer;

        internal new static App Current;

        /// <summary>
        /// CollectionObserver实例
        /// </summary>
        internal ObservableCollectionVM collectionObserver { get; private set; }

        private async Task Initial ()
        {
#if DEBUG
            //await Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
#endif
            DatabaseController.Migrate();

            await EnsureChildTemporaryFolders(Covers.ToString() , Filters.ToString());

            Helpers.CoverHelper.InitialDefaultCover();

            collectionObserver.ErrorZipEvent += (string str) =>
            {
                new ToastContentBuilder()
                      .AddText($"{str}\r{ResourceLoader.GetForCurrentView("StringResources").GetString("ErrorString1")}")
                      .Show();
            };

            collectionObserver.WorkDoneEvent += (string str) =>
            {
                new ToastContentBuilder()
                    .AddText(str)
                    .Show();
            };      

            var folder = MyUWPLibrary.AccestListHelper.GetAvailableFutureFolder().Result.ToArray();

            await   collectionObserver.Initialize(folder);

        }

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行， 已执行，逻辑上等同于 main() 或
        /// WinMain()。
        /// </summary>
        public App ()
        {
            this.InitializeComponent();
            collectionObserver = new ObservableCollectionVM();

            Current = this;
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e"> 有关启动请求和过程的详细信息。 </param>
        protected override async void OnLaunched (LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化， 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置 参数
                    rootFrame.Navigate(typeof(MainPage) , e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }

            await Initial();

        }

        /// <summary> </summary>
        /// <remarks> 不能瞎改，里面一些奇葩问题 </remarks>
        /// <param name="args"> </param>
        protected override void OnFileActivated (FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);

            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                Window.Current.Content = rootFrame;
            }

            // 这一步不能删，搞不懂为什么，删了就无法正常使用
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage));
            }

            var file = args.Files[0] as Windows.Storage.StorageFile;
            MangaBook mangaBook = new MangaBook(file , null );
            mangaBook.Cover = CoverHelper.DefaultCover;
            rootFrame.Navigate(typeof(ReadPage) , mangaBook);

            Window.Current.Activate();
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        private void OnNavigationFailed (object sender , NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。 在不知道应用程序 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender"> 挂起的请求的源。 </param>
        /// <param name="e"> 有关挂起请求的详细信息。 </param>
        private void OnSuspending (object sender , SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        
    }
}