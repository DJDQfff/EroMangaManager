namespace EroMangaManager.Core.Models;

/// <summary>
/// 用于导航到搜索页面的数据包
/// </summary>
public struct SearchParameter
{
    /// <summary>
    /// 本子名
    /// </summary>
    public string MangaName { set; get; }

    /// <summary>
    /// 标签
    /// </summary>
    public IEnumerable<string> Tags { set; get; }
}