using System;
using System.Collections.Generic;
using System.Linq;

namespace Database;

public partial class DatabaseController
{
    /// <summary> 查询数据库，获取Tag键值对的信息 </summary>
    /// <param name="tags"> </param>
    /// <returns>
    /// 一个字典，第一项为本子标签，第二项为对应的TagName（如没有则为空字符串）
    /// </returns>
    public Dictionary<string, string> MatchTag(IEnumerable<string> tags)
    {
        using var database = contextFactory.CreateDbContext();
        var dictionaries = this.TagCategory_QueryAll();

        Dictionary<string, string> keyValuePairs = [];

        tags = [.. tags.Distinct()]; // 去重

        foreach (var tag in tags)
        {
            string? key = dictionaries.FirstOrDefault(d => d.Value.Contains(tag)).Key;

            keyValuePairs.Add(tag, key);
        }
        return keyValuePairs;
    }
}
