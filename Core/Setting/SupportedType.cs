namespace EroMangaManager.Core.Setting;

/// <summary>
/// 支持的格式
/// </summary>
public class SupportedType
{
    /// <summary>
    /// 空字符串是文件夹
    /// </summary>
    public static string[] MangaType => [string.Empty, ".zip", ".7z", ".rar", ".cbz", "cbr", "cb7"];

    /// <summary>
    /// 内部支持的图片格式
    /// </summary>
    public static string[] ImageType => [".png", ".bmp", ".jpg", ".jpeg", ".webp"];
}