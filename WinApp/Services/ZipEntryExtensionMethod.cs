using Core.Setting;

namespace Core.Services;

/// <summary>
///
/// </summary>
public static class ZipEntryExtensionMethod
{

    /// <summary>
    /// 筛选zipentry，如果filteredImages为null，则不进行比较
    /// </summary>
    /// <param name="entry"></param>
    /// <param name="filteredImages">要进行比较的数据，如果为null，则不进行数据比较</param>
    /// <returns></returns>
    public static bool EntryFilter (this IArchiveEntry entry , FilteredImage[]? filteredImages)
    {
        bool canuse = true;

        if (entry.IsDirectory) // 排除文件夹entry
            return false;

        var extension = Path.GetExtension(entry.Key!).ToLower();

        if (!SupportedType.ImageType.Contains(extension))
        {
            return false;
        }

        if (filteredImages != null) // 是否检查图片过滤
        {
            #region 第一个条件：比较数据库，解压后大小

            if (!filteredImages.Any(n => n.ZipEntryLength == entry.Size))
                return true;

            #endregion 第一个条件：比较数据库，解压后大小

            #region // 第二个条件： 计算流hash，判断唯一性

            string hash = entry.ComputeHash();

            int count = filteredImages.Count(n => n.Hash == hash);

            if (count != 0)
            {
                canuse = false;
            }

            #endregion // 第二个条件： 计算流hash，判断唯一性
        }

        return canuse; // 最后一定符合调教
    }

}
