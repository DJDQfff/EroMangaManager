namespace Core.ViewModels;

/// <summary>
/// 筛选参数。UI 上每个条件有一个"是否启用"开关，未启用的条件不参与过滤。
/// </summary>
public partial class MangaFilterOptions : ObservableObject
{
    // 名称（匹配 Name 或 TranslatedName）
    [ObservableProperty]
    public partial bool FilterByName { get; set; }

    [ObservableProperty]
    public partial string NameKeyword { get; set; } = "";

    // 文件大小（单位：字节；Max 为 0 表示不限）
    [ObservableProperty]
    public partial bool FilterByFileSize { get; set; }

    [ObservableProperty]
    public partial long MinFileSize { get; set; }

    [ObservableProperty]
    public partial long MaxFileSize { get; set; }

    // 文件类型（空字符串=文件夹，UI 用 "__folder__" 哨兵）

    // 类型（永远启用，至少一个）
    public ObservableCollection<string> FileTypes { get; } = ["__folder__", ".zip"];

    // 标签（逗号分隔，任一命中即算）
    [ObservableProperty]
    public partial bool FilterByTag { get; set; }

    [ObservableProperty]
    public partial string TagKeyword { get; set; } = "";

    /// <summary>是否至少启用了一个条件</summary>
    public bool HasAny => FilterByName || FilterByFileSize || FilterByTag;

    public void ClearAll()
    {
        FilterByName = false;
        NameKeyword = "";

        FilterByFileSize = false;
        MinFileSize = 0;
        MaxFileSize = 0;

        FileTypes.Clear();
        FileTypes.Add("__folder__");
        FileTypes.Add(".zip");

        FilterByTag = false;
        TagKeyword = "";
    }
}
