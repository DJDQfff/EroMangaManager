namespace EroMangaManager.Core.Models;

/// <summary> 本子 </summary>
public partial class Manga : ObservableObject
{
    public  string Guid { get; set; }
    /// <summary>
    /// 封面文件路径
    /// </summary>
    /// <remarks>这个一定要有，不能为null，不然在Image控件加载图像时会异常导致程序闪退</remarks>
    [ObservableProperty]
    private string coverPath;

    /// <summary>
    /// 内里多少图像文件
    /// </summary>
    [ObservableProperty]
    private int imageAmount;

    /// <summary> 漫画文件路径 </summary>
    [ObservableProperty]
    private string filePath;
    
    /// <summary>
    /// 章节。每个含有图片的文件夹视为一个chapter。仅在快速统计章节量时使用，应使用chapters属性
    /// </summary>
    [ObservableProperty]
    private int chapterAmount;

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

        MangaName = string.Join(' ', BracketBasedStringParser.Get_OutsideContent(FileDisplayName));
        Tags = BracketBasedStringParser.Get_InsideContent(FileDisplayName).SelectMany(x => x.Split('&', '、')).Distinct().ToArray();

        FolderPath = Path.GetDirectoryName(FilePath);
        FileFullName = Path.GetFileName(FilePath);
    }

    /// <summary> 漫画翻译后的名称 </summary>
    [ObservableProperty]
    private string translatedMangaName;    //TODO 翻译漫画名的功能

    /// <summary> 实例化EroManga </summary>
    public Manga(string filepath)
    {
        FilePath = filepath;
    }

    /// <summary>
    ///本子类型，可以为.zip .7z 文件夹，这也是对应的文件后缀名，文件夹的话为“”空字符串
    /// </summary>
    [ObservableProperty]
    private string type;

    /// <summary> 文件Display名（不带扩展名） </summary>
    [ObservableProperty]
    private string fileDisplayName;

    /// <summary> 漫画文件名（全名，带扩展名，不包含文件夹名） </summary>
    [ObservableProperty]
    private string fileFullName;

    /// <summary>
    /// 获取漫画文件大小。单位：字节
    /// </summary>
    [ObservableProperty]
    private long fileSize;

    /// <summary> 漫画文件所在文件夹路径 </summary>
    [ObservableProperty]
    private string folderPath;

    /// <summary> 本子名字。第一个括号外的内容（括号外内容可能有多个,也可能所有内容都在括号内） </summary>
    [ObservableProperty]
    private string mangaName;

    /// <summary> 文件名中包含在括号的本子Tag </summary>
    [ObservableProperty]
    private string[] tags;

    //Chapter属性循环引用（引用回了Manga）导致json序列化出错
    [System.Text.Json.Serialization.JsonIgnore]
    public ObservableCollection<Chapter> Chapters { get; } = [];
}