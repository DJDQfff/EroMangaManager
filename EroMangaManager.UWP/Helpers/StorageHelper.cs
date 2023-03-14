using System;
using System.Threading.Tasks;

using EroMangaManager.UWP.SettingEnums;
using EroMangaManager.UWP.Views.ContentDialogPages;
using EroMangaManager.Core.Models;
using Windows.Storage;

using Windows.UI.Xaml.Controls;

namespace EroMangaManager.UWP.Helpers
{
    /// <summary>
    /// 为什么要弄这个：
    /// 因为删除文件时，除了直接删除，还有涉及和程序交互的部分、读取程序设置的部分
    /// </summary>
    internal class StorageHelper
    {
        /// <summary>
        /// 删除源文件时，会触发删除确认弹框，删除模式，这两个参数都是从程序设置中读取的，因此封装到助手类里面
        /// </summary>
        /// <param name="eroManga"></param>
        /// <returns></returns>
        public static async Task DeleteSourceFile (MangaBook eroManga)
        {
            var temp1 = (bool) (ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.WhetherShowDialogBeforeDelete.ToString()] ?? false);
            var temp2 = (bool) (ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.StorageDeleteOption.ToString()] ?? false);

            var option = temp2 ? StorageDeleteOption.PermanentDelete : StorageDeleteOption.Default;

            if (!temp1)
            {
                ConfirmDeleteMangaFile confirm = new ConfirmDeleteMangaFile(eroManga);
                var result = await confirm.ShowAsync();
                switch (result)
                {
                    case ContentDialogResult.Primary:
                         App.Current.GlobalViewModel.RemoveManga(eroManga );
                        await App.Current.storageItemManager.DeleteStorageFile(eroManga.FilePath , option);
                        break;

                    case ContentDialogResult.Secondary:
                        break;
                }
            }
            else
            {
                 App.Current.GlobalViewModel.RemoveManga(eroManga );
                await App.Current.storageItemManager.DeleteStorageFile(eroManga.FilePath , option);

            }
        }
    }
}