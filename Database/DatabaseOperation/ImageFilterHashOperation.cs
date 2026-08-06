using System;
using System.Linq;
using System.Threading.Tasks;
using Database.Entities;

namespace Database;

public partial class DatabaseController
{
    public FilteredImage[] FilteredImageArray()
    {
        using var database = contextFactory.CreateDbContext();
        return [.. database.FilteredImages];
    }

    /// <summary>
    /// 统计符合Length条件的个数
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public int ImageFilter_LengthConditionCount(long length)
    {
        using var database = contextFactory.CreateDbContext();
        var query = database.FilteredImages.Count(n => n.ZipEntryLength == length);

        return query;
    }

    /// <summary>
    /// 统计符合Hash条件的个数
    /// </summary>
    /// <param name="hash"></param>
    /// <returns></returns>
    public int ImageFilter_HashConditionCount(string hash)
    {
        using var database = contextFactory.CreateDbContext();
        var query = database.FilteredImages.Count(n => n.Hash == hash);

        return query;
    }

    /// <summary>
    /// ImageFilter表添加新行
    /// </summary>
    /// <param name="hash"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public async Task ImageFilter_Add(string hash, long length)
    {
        using var database = contextFactory.CreateDbContext();
        FilteredImage imageHash = new() { Hash = hash, ZipEntryLength = length };
        database.Add(imageHash);
        await database.SaveChangesAsync();
    }

    /// <summary>
    /// ImageFIlter表移除行
    /// </summary>
    /// <param name="hashes"></param>
    /// <returns></returns>
    public async Task ImageFilter_Remove(string[] hashes)
    {
        using var database = contextFactory.CreateDbContext();
        var h = database.FilteredImages.Where(n => hashes.Contains(n.Hash)).ToArray();
        database.RemoveRange(h);
        await database.SaveChangesAsync();
    }
}
