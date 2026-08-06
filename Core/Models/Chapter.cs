using System.Text.Json.Serialization;

namespace Core.Models;

/// <summary>
/// 章节，Manga内容分成若干章节
/// </summary>
public class Chapter
{
    /// <summary>
    /// 源manga
    /// </summary>
    [JsonIgnore]
    public Manga? Manga { get; init; }

    /// <summary>
    ///
    /// </summary>
    public Chapter() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="manga"></param>
    public Chapter(Manga manga)
    {
        Manga = manga;
    }

    /// <summary>
    /// 章节名
    /// </summary>
    public required string Chaptername { get; init; }

    /// <summary>
    /// 获取各章节的key。
    /// 用于配合 <see cref="MangaStreamProvider"/> 使用，以前是在一起的，为序列化拆开。
    /// </summary>
    public List<string> Chapterimagekey { get; init; } = [];
}
