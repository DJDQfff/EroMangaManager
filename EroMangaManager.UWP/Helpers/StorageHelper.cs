using System;
using System.IO;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.SettingEnums;
using EroMangaManager.UWP.Views.ContentDialogPages;

using Windows.Storage;
using Windows.UI.Xaml.Controls;

using static EroMangaManager.UWP.SettingEnums.General;

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
        /// <returns></returns>
        public static async Task RenameSourceFile(MangaBook eroManga)
        {
            // TODO 等EditTag功能好了，在改回EditTag页面
            var renameDialog = new RenameDialog(eroManga);

            var result = await renameDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // text是否合法由对话框保证
                var text = renameDialog.NewDisplayName;

                await MyLibrary.UWP.AccestListHelper.RenameFile(eroManga.FilePath, text + ".zip");

                eroManga.MangaName = text;
                eroManga.FilePath = Path.Combine(eroManga.FolderPath, text + ".zip");

                eroManga.NotifyPropertyChanged(string.Empty);
            }
        }

        /// <summary>
        /// 删除源文件时，会触发删除确认弹框，删除模式，这两个参数都是从程序设置中读取的，因此封装到助手类里面
        /// </summary>
        /// <param name="eroManga"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteSourceFile(MangaBook eroManga)
        {
            var temp1 = App.Current.AppConfig[nameof(General)][nameof(WhetherShowDialogBeforeDelete)].BoolValue;

            var temp2 = App.Current.AppConfig[nameof(General)][nameof(StorageFileDeleteOption)].BoolValue;

            var option = temp2 ? StorageDeleteOption.PermanentDelete : StorageDeleteOption.Default;

            bool deleteResult = false;
            if (!temp1)
            {
                ConfirmDeleteMangaFile confirm = new ConfirmDeleteMangaFile(eroManga);
                var result = await confirm.ShowAsync();
                switch (result)
                {
                    case ContentDialogResult.Primary:
                        App.Current.GlobalViewModel.RemoveManga(eroManga);
                        await MyLibrary.UWP.AccestListHelper.DeleteStorageFile(eroManga.FilePath, option);
                        deleteResult = true;
                        break;

                    case ContentDialogResult.Secondary:
                        break;
                }
            }
            else
            {
                App.Current.GlobalViewModel.RemoveManga(eroManga);
                await MyLibrary.UWP.AccestListHelper.DeleteStorageFile(eroManga.FilePath, option);
                deleteResult = true;
            }

            return deleteResult;
        }
    }
}