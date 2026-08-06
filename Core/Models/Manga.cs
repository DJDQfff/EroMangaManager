using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Core.Models;

/// <summary> 本子 </summary>
/// <remarks> 实例化EroManga </remarks>
public partial class Manga : ObservableObject /*,IMangaView*/
{
    /// <summary>
    ///
    /// </summary>
    public string Guid { get; set; } = string.Empty!;

    /// <summary>
    /// 封面文件路径
    /// </summary>
    /// <remarks>这个一定要有，不能为null，不然在Image控件加载图像时会异常导致程序闪退</remarks>
    [ObservableProperty]
    public partial string CoverUri { get; set; } = string.Empty!;

    /// <summary>
    /// 内里多少图像文件
    /// </summary>
    [ObservableProperty]
    public partial int ImageAmount { get; set; }

    /// <summary> 漫画文件路径 </summary>
    [ObservableProperty]
    [JsonIgnore]
    public partial string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 章节。避免使用。每个含有图片的文件夹视为一个chapter。仅在快速统计章节量时使用，应使用chapters.Count属性
    /// </summary>
    [ObservableProperty]
    public partial int ChapterAmount { get; set; }

    /// <summary> 实例化EroManga </summary>
    public Manga() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="filepath"></param>
    public Manga(string filepath)
    {
        FilePath = filepath;
    }

    partial void OnFilePathChanged(string value)
    {
        if (Directory.Exists(FilePath))
        {
            FileDisplayName = Path.GetFileName(FilePath);
            Type = string.Empty;
        }
        if (File.Exists(FilePath))
        {
            FileDisplayName = Path.GetFileNameWithoutExtension(FilePath);
            Type = Path.GetExtension(value).ToLower();
        }

        Name = string.Join(' ', BracketBasedStringParser.Get_OutsideContent(FileDisplayName));
        Tags = BracketBasedStringParser
            .Get_InsideContent(FileDisplayName)
            .SelectMany(x => x.Split('&', '、'))
            .Distinct()
            .ToArray();

        var dir = Path.GetDirectoryName(FilePath);
        if (dir is null)
            throw new InvalidOperationException($"无法从文件路径提取目录名: '{FilePath}'");

        FolderPath = dir; // ✅ 编译器通过流分析知道此处 dir 非空，无警告FolderPath = Path.GetDirectoryName(FilePath);
        FileFullName = Path.GetFileName(FilePath);
    }

    /// <summary> 漫画翻译后的名称 </summary>
    [ObservableProperty]
    public partial string TranslatedName { get; set; } = string.Empty;

    /// <summary>
    ///本子类型，可以为.zip .7z 文件夹，这也是对应的文件后缀名，文件夹的话为“”空字符串
    /// </summary>
    [ObservableProperty]
    public partial string Type { get; set; } = string.Empty;

    /// <summary> 文件Display名（不带扩展名） </summary>
    [ObservableProperty]
    [JsonIgnore]
    public partial string FileDisplayName { get; set; } = string.Empty;

    /// <summary> 漫画文件名（全名，带扩展名，不包含文件夹名） </summary>
    [ObservableProperty]
    [JsonIgnore]
    public partial string FileFullName { get; set; } = string.Empty;

    /// <summary>
    /// 获取漫画文件大小。单位：字节
    /// </summary>
    [ObservableProperty]
    public partial long FileSize { get; set; } = 0;

    /// <summary> 漫画文件所在文件夹路径 </summary>
    [ObservableProperty]
    [JsonIgnore]
    public partial string FolderPath { get; set; } = string.Empty;

    /// <summary> 本子名字。第一个括号外的内容（括号外内容可能有多个,也可能所有内容都在括号内） </summary>
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    /// <summary> 文件名中包含在括号的本子Tag </summary>
    [ObservableProperty]
    public partial string[] Tags { get; set; } = [];

    /// <summary>
    /// 内章节
    /// </summary>
    public ObservableCollection<Chapter> Chapters { get; set; } = [];
}
