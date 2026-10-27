using System.Security.Cryptography;

namespace UnoLibrary.Services;

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

public static bool EntryFilter(this IArchiveEntry entry, FilteredImage[]? filteredImages)
{
    if (entry.IsDirectory)
        return false;

    var extension = Path.GetExtension(entry.Key!).ToLower();
    if (!SupportedType.ImageType.Contains(extension))
        return false;

    if (filteredImages != null)
    {
        // 第一个条件：解压后大小
        if (!filteredImages.Any(n => n.ZipEntryLength == entry.Size))
            return true;

        // 第二个条件：SHA256 比对唯一性
        string hash;
        using (var stream = entry.OpenEntryStream())
        using (var sha256 = SHA256.Create())
        {
            byte[] vs = sha256.ComputeHash(stream);
            hash = Convert.ToHexString(vs);
        }

        if (filteredImages.Any(n => n.Hash == hash))
            return false;
    }

    return true;
}}
