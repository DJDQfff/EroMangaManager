﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.Core.ViewModels;
using EroMangaManager.UWP.Helpers;
using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.SettingEnums;
using EroMangaManager.UWP.Views;

using Microsoft.Toolkit.Uwp.Notifications;

using SharpConfig;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using static EroMangaDB.BasicController;
using static EroMangaManager.UWP.SettingEnums.FolderEnum;
using static EroMangaManager.UWP.SettingEnums.General;
using static MyLibrary.UWP.StorageFolderHelper;

namespace EroMangaManager.UWP
{
    /// <summary> 提供特定于应用程序的行为，以补充默认的应用程序类。 </summary>
    public sealed partial class App : Application
    {
        internal new static App Current;

        /// <summary>
        /// 全局ViewModel
        /// </summary>
        internal ObservableCollectionVM GlobalViewModel { get; private set; }

        internal Configuration AppConfig { get; private set; }
        internal string AppConfigPath { get; private set; }

        private async Task QuickInitialWork()
        {
#if DEBUG
            await Windows.System.Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
#endif

            DatabaseController.Migrate();

            //Windows.ApplicationModel.Resources.Core.ResourceContext.SetGlobalQualifierValue("Language", "fr");

            EnsureAppConfigFile();

            Helpers.CoverHelper.InitialDefaultCover();
            await EnsureChildTemporaryFolders(Covers.ToString(), Filters.ToString());

            #region 事件赋值

            GlobalViewModel.ErrorZipEvent += (string str) =>
            {
                new ToastContentBuilder()
                      .AddText($"{str}\r{ResourceLoader.GetForViewIndependentUse("StringResources").GetString("ErrorString1")}")
                      .Show();
            };

            GlobalViewModel.WorkDoneEvent += (string str) =>
            {
                new ToastContentBuilder()
                    .AddText(str)
                    .Show();
            };

            #endregion 事件赋值

            #region 初始化文件夹目录

            var alldic = await MyLibrary.UWP.AccestListHelper.GetAvailableFutureFolder();

            Dictionary<string, StorageFolder> keyValuePairs = new Dictionary<string, StorageFolder>();

            var folders = Configuration.LoadFromFile(AppConfigPath)[nameof(General)][nameof(LibraryFolders)].StringValueArray;

            foreach (var folder in folders)
            {
                try
                {
                    var pair = alldic.Single(x => x.Value.Path == folder);

                    keyValuePairs.Add(pair.Key, pair.Value);
                }
                catch { }
            }

            ModelFactory.ViewModelGetAllFolders(GlobalViewModel, keyValuePairs.Values);

            #endregion 初始化文件夹目录
        }

        /// <summary>
        /// 创建设置文件
        /// </summary>
        private void EnsureAppConfigFile()
        {
            var filename = "AppConfig.ini";
            var localfolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            AppConfigPath = Path.Combine(localfolder.Path, filename);

            if (!File.Exists(AppConfigPath))
            {
                AppConfig = new Configuration();
                AppConfig[nameof(General)][nameof(IsFilterImageOn)].BoolValue = false;
                AppConfig[nameof(General)][nameof(WhetherShowDialogBeforeDelete)].BoolValue = true;
                AppConfig[nameof(General)][nameof(StorageFileDeleteOption)].BoolValue = false;
                AppConfig[nameof(General)][nameof(LibraryFolders)].StringValueArray = new string[] { };

                AppConfig.SaveToFile(AppConfigPath);
            }
            else
            {
                AppConfig = Configuration.LoadFromFile(AppConfigPath);
            }
        }

        private async Task LongTimeLoad()
        {
            await ModelFactory.ViewModelInitialEachFolders(GlobalViewModel);
        }

        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行， 已执行，逻辑上等同于 main() 或
        /// WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            GlobalViewModel = new ObservableCollectionVM();

            Current = this;
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e"> 有关启动请求和过程的详细信息。 </param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            await QuickInitialWork();
            // 不要在窗口已包含内容时重复应用程序初始化， 只需确保窗口处于活动状态
            if (!(Window.Current.Content is Frame rootFrame))
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
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }

            await LongTimeLoad();
        }

        /// <summary> </summary>
        /// <remarks> </remarks>
        /// <param name="args"> </param>
        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);
            if (AppConfig is null)
            {
                EnsureAppConfigFile();
            }

            var file = args.Files[0] as StorageFile;

            switch (Window.Current.Content)
            {
                case Frame rootFrame:
                    MangaBook book = await ModelFactory.CreateMangaBook(file);

                    await WindowHelper.ShowNewWindow(typeof(ReadPage), book);

                    break;

                case null:
                    {
                        var rootFrame = new Frame();
                        rootFrame.NavigationFailed += OnNavigationFailed;
                        rootFrame.Navigate(typeof(ReadPage), file);
                        var title = ApplicationView.GetForCurrentView().Title = file.Name;
                        Window.Current.Content = rootFrame;
                        Window.Current.Closed += (objectsender, sss) =>
                        {
                            var page = rootFrame.Content as ReadPage;

                            page.currentReader?.Dispose();
                            GC.SuppressFinalize(Window.Current);
                        };

                        Window.Current.Activate();
                    }

                    break;
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。 在不知道应用程序 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender"> 挂起的请求的源。 </param>
        /// <param name="e"> 有关挂起请求的详细信息。 </param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }
    }
}