namespace UnoLibrary.Services;

// TODO 改成使用ms-appx的uri来使用同步，以在调用方使用valuetask，减少无意义的任务状态机
/// <summary>
/// 应用程序数据文件夹帮助类
/// </summary>
public class StorageFolderHelper
{
    public StorageFolderHelper()
    {
        EnsureTemporaryFolderChild_Covers("Covers", "Filters");
    }

    /// <summary> 在app临时文件夹获取子文件夹 </summary>
    /// <param name="foldername"> 子文件夹的名称 </param>
    /// <returns> </returns>
    public async Task<StorageFolder> GetChildTemporaryFolder(string foldername)
    {
        var LocalCacheFolder = ApplicationData.Current.TemporaryFolder;

        var CoversFolder = await LocalCacheFolder.GetFolderAsync(foldername);

        return CoversFolder;
    }

    /// <summary>
    /// 确保在app临时文件夹存在文件夹，如果不存在，则创建文件夹
    /// </summary>
    /// <param name="foldernames"> 子文件夹名称 </param>
    /// <returns> </returns>
    public void EnsureTemporaryFolderChild_Covers(params string[] foldernames)
    {
        var temporaryfolderpath = ApplicationData.Current.TemporaryFolder.Path;

        foreach (var folder in foldernames)
        {
            var newfolder = Path.Combine(temporaryfolderpath, folder);
            Directory.CreateDirectory(newfolder);
        }
    }

    /// <summary>
    /// 查询指定StorageItems集合中是否存在相同路径的StorageItem
    /// </summary>
    /// <param name="Items"></param>
    /// <param name="check"> </param>
    /// <returns> </returns>
    public bool Contain(IEnumerable<IStorageItem> Items, StorageFolder check)
    {
        if (Items.Any(x => x.Path == check.Path))
        {
            return true;
        }
        return false;
    }
}
