namespace EroMangaManager.Core.ViewModels;

/// <summary>
/// 表示更新状态
/// </summary>
public enum MangasGroupUpdateState
{
    /// <summary>
    /// 等待开始
    /// </summary>
    Ready,

    /// <summary>
    /// 更新中
    /// </summary>
    Busy,

    /// <summary>
    /// 结束
    /// </summary>
    Over,
}
