using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Entities;

namespace Database;

public partial class DatabaseController
{
    /// <summary>
    /// ReadingInof表添加多个
    /// </summary>
    /// <param name="ts"></param>
    /// <returns></returns>
    public async Task ReadingInfo_AddMulti(IEnumerable<ReadingInfo> ts)
    {
        using var database = contextFactory.CreateDbContext();
        await database.AddRangeAsync(ts);
        await database.SaveChangesAsync();
    }

    /// <summary>
    /// ReadingInfo移除单个
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public async Task ReadingInfo_RemoveSingle(string path)
    {
        using var database = contextFactory.CreateDbContext();
        var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == path);
        if (info != null)
        {
            database.Remove(info);
            await database.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ReadingInfo更新单个
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public async Task ReadingInfo_UpdateSingle(ReadingInfo t)
    {
        using var database = contextFactory.CreateDbContext();
        var info = database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == t.AbsolutePath);
        if (info != null)
        {
            database.Remove(info);
            database.Add(info);
            await database.SaveChangesAsync();
        }
    }

    /// <summary>
    /// ReadingInfo表更新Name属性
    /// </summary>
    /// <param name="readinginfo"></param>
    /// <param name="manganame"></param>
    /// <returns></returns>
    public async Task ReadingInfo_UpdateName(ReadingInfo readinginfo, string manganame)
    {
        using var database = contextFactory.CreateDbContext();
        // ✅ 明确处理未找到的情况
        var info =
            database.ReadingInfos.SingleOrDefault(n => n.AbsolutePath == readinginfo.AbsolutePath)
            ?? throw new InvalidOperationException(
                $"未找到路径为 '{readinginfo.AbsolutePath}' 的阅读记录"
            );
        info.Name = manganame;
        database.Update(info);
        await database.SaveChangesAsync();
    }

    /// <summary>
    /// ReadingInfo表修改多个翻译本子名属性
    /// </summary>
    /// <param name="tuples"></param>
    /// <returns></returns>
    public async Task ReadingInfo_MultiTranslateName(
        IEnumerable<(string path, string translatedname)> tuples
    )
    {
        using var database = contextFactory.CreateDbContext();
        var info = database.ReadingInfos.ToList();
        foreach (var (path, translatedname) in tuples)
        {
            var a =
                info.SingleOrDefault(n => n.AbsolutePath == path)
                ?? throw new InvalidOperationException($"未找到路径为 '{path}' 的阅读记录");
            a.Name_Translated = translatedname;
        }
        database.UpdateRange(info);
        await database.SaveChangesAsync();
    }

    /// <summary>
    /// ReadingInfo表查询所有
    /// </summary>
    /// <returns></returns>
    public ReadingInfo[] ReadingInfo_QueryAll()
    {
        using var database = contextFactory.CreateDbContext();
        var infos = database.ReadingInfos.ToArray();

        return infos;
    }
}
