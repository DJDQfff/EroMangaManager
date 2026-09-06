namespace WinApp.Services;

/// <summary>
/// 需要通过对话框用户进行交互，以及一些读取程序设置的操作
/// </summary>
public class DialogHelper(
    Window window,
    StorageOperation storageOperation,
    SettingViewModel setting
)
{
    /// <summary>
    /// 修改文件名
    /// </summary>
    /// <param name="eroManga"></param>
    /// <returns></returns>
    public async Task RenameSourceFileInDialog(Manga eroManga)
    {
        // TODO 暂时放弃，不会写页面UI，写出来也丑。等EditTag功能好了，在改回EditTag页面
        RenameDialog renameDialog = new(eroManga) { XamlRoot = window.Content.XamlRoot };

        _ = await renameDialog.ShowAsync();
    }

    /// <summary>
    /// 删除源文件时，会触发删除确认弹框，删除模式，这两个参数都是从程序设置中读取的，因此封装到助手类里面
    /// </summary>
    /// <param name="eroManga"></param>
    /// <returns></returns>
    public async Task<bool> ConfirmDeleteSourceFileDialog(Manga eroManga)
    {
        var temp1 = setting.AppConfig.General.WhetherShowDialogBeforeDelete;

        var temp2 = setting.AppConfig.General.StorageFileDeleteOption;

        var deletemode = temp2 ? StorageDeleteOption.PermanentDelete : StorageDeleteOption.Default;

        if (!temp1)
        {
            ConfirmDeleteMangaFile confirm = new(eroManga) { XamlRoot = window.Content.XamlRoot };
            var result = await confirm.ShowAsync();
            switch (result)
            {
                case ContentDialogResult.Primary:
                    await storageOperation.Delete(eroManga, deletemode);
                    return true;

                default:
                    return false;
            }
        }
        else
        {
            await storageOperation.Delete(eroManga, deletemode);

            return true;
        }
    }
}
