﻿using System;
using System.IO;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.SettingEnums;
using EroMangaManager.UWP.Views.ContentDialogPages;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace EroMangaManager.UWP.Helpers
{
    /// <summary>
    /// 操作文件时，还要与用户进行交互，以及一些别的操作
    /// </summary>
    internal class StorageHelper
    {
        /// <summary>
        /// 修改文件名
        /// </summary>
        /// <param name="eroManga"></param>
        /// <param name="suggestedname"></param>
        /// <returns></returns>
        public static async Task RenameSourceFile(MangaBook eroManga, string suggestedname)
        {
            RenameDialog renameDialog = new RenameDialog(eroManga, suggestedname);
            var result = await renameDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // text是否合法由对话框保证
                var text = renameDialog.NewDisplayName;

                await App.Current.storageItemManager.RenameStorageFile(eroManga.FilePath, text + ".zip");
                eroManga.MangaName = text;
                eroManga.FilePath = Path.Combine(eroManga.FolderPath, text + ".zip");

                eroManga.NotifyPropertyChanged(string.Empty);
                // TODO 不知道为什么，写FilePath属性时是调用过一次次方法的，但是那样不会起作用，还得在外部再调用一次

                // TODO 这里MangaBook的Tag属性页因该同步更新，但是算了。
                // 还是把SplitAndParser方法成两个，每次按需调用
            }
        }

        /// <summary>
        /// 删除源文件时，会触发删除确认弹框，删除模式，这两个参数都是从程序设置中读取的，因此封装到助手类里面
        /// </summary>
        /// <param name="eroManga"></param>
        /// <returns></returns>
        public static async Task DeleteSourceFile(MangaBook eroManga)
        {
            var temp1 = (bool)(ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.WhetherShowDialogBeforeDelete.ToString()] ?? false);
            var temp2 = (bool)(ApplicationData.Current.LocalSettings.Values[ApplicationSettingItemName.StorageDeleteOption.ToString()] ?? false);

            var option = temp2 ? StorageDeleteOption.PermanentDelete : StorageDeleteOption.Default;

            if (!temp1)
            {
                ConfirmDeleteMangaFile confirm = new ConfirmDeleteMangaFile(eroManga);
                var result = await confirm.ShowAsync();
                switch (result)
                {
                    case ContentDialogResult.Primary:
                        App.Current.GlobalViewModel.RemoveManga(eroManga);
                        await App.Current.storageItemManager.DeleteStorageFile(eroManga.FilePath, option);
                        break;

                    case ContentDialogResult.Secondary:
                        break;
                }
            }
            else
            {
                App.Current.GlobalViewModel.RemoveManga(eroManga);
                await App.Current.storageItemManager.DeleteStorageFile(eroManga.FilePath, option);
            }
        }
    }
}