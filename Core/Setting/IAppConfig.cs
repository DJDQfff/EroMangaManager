namespace EroMangaManager.Core.Setting;

/// <summary>
/// 设置项总和
/// </summary>
public interface IAppConfig
{
    /// <summary>
    /// 通用设置
    /// </summary>
    General General { get; set; }

    /// <summary>
    /// 漫画打开方式
    /// </summary>
    MangaOpenWay3 MangaOpenWay3 { get; set; }
}

/// <summary>
/// 一般设置项
/// </summary>
#pragma warning disable IDE1006 // 命名样式

public interface General
#pragma warning restore IDE1006 // 命名样式
{
    /// <summary>
    /// 是否启用过滤图片功能
    /// </summary>
    [DefaultValue(false)]
    bool IsFilterImageOn { set; get; }

    /// <summary>
    /// 是否在删除前显示对话框
    /// </summary>
    [DefaultValue(true)]
    bool WhetherShowDialogBeforeDelete { set; get; }

    /// <summary>
    /// 文件删除设置
    /// </summary>
    [DefaultValue(false)]
    bool StorageFileDeleteOption { set; get; }

    /// <summary>
    /// 默认图书书架
    /// </summary>
    string DefaultBookcaseFolder { set; get; }

    /// <summary>
    /// 项删除
    /// </summary>
    [DefaultValue(false)]
    bool StorageDeleteOption { set; get; }

    /// <summary>
    /// 是否显示空文件夹
    /// </summary>
    [DefaultValue(false)]
    bool IsEmptyFolderShow { set; get; }

    /// <summary>
    /// app默认UI语言
    /// </summary>
    [Obsolete("使用LanguageIndex", true)]
    [DefaultValue(DefaultAppUILanguageStrings.zhCN)]
    string DefaultAppUILanguage { set; get; }

    /// <summary>
    /// 语言序数
    /// </summary>
    [DefaultValue(0)]
    int LanguageIndex { set; get; }

    /// <summary>
    /// 系统主题
    /// </summary>
    [DefaultValue(ThemeValue.Auto)]
    int Theme { set; get; }

    /// <summary>
    /// 默认书架界面
    /// </summary>
    [DefaultValue(null)]
    string BookcaseTemplateKey { set; get; }

    /// <summary>
    /// 是否加载子文件夹
    /// </summary>
    [DefaultValue(false)]
    bool WhetherPickSubFolder { set; get; }
}

/// <summary>
/// app默认UI语言
/// </summary>
public class DefaultAppUILanguageStrings
{
    /// <summary>
    /// 简中
    /// </summary>
    public const string zhCN = "zh-CN";

    /// <summary>
    /// 英文
    /// </summary>
    public const string en = "en";
}

/// <summary>
/// 新的漫画打开方式设置，旧的不再使用
/// </summary>
#pragma warning disable IDE1006 // 命名样式

public interface MangaOpenWay3
#pragma warning restore IDE1006 // 命名样式
{
    /// <summary>
    /// 绑定序数
    /// </summary>
    [DefaultValue(0)]
    int WayIndex { set; get; }

    /// <summary>
    /// 打开方式字符串都放这里，因为config.net还不支持集合，用这个代替，需要在外部再套一层集合操作
    /// ?的两个为默认设置，|后面跟的是打开exe
    /// </summary>
    [DefaultValue("?InternalReadPage?OSRelated?explorer.exe")]
    string OpenWays { set; get; }
    /// <summary>
    /// 默认exe，注意，在MangaeMangaOpenWay3Setting.xaml控件中，RadioButtons无法通过此设置数据绑定（双向不可以，单向不知道），额外通过wayindex来绑定。
    /// </summary>
    [DefaultValue("explorer.exe")]
    string DefaultWay { set; get; }
}

/// <summary>
/// 漫画打开方式相关设置
/// </summary>
#pragma warning disable IDE1006 // 命名样式

public interface MangaOpenWaySetting
#pragma warning restore IDE1006 // 命名样式
{
    /// <summary>
    /// 打开漫画方式
    /// </summary>
    [DefaultValue(ReadMangaWayStrings.InternalReadPage)]
    string ReadMangaWay { set; get; }

    /// <summary>
    /// 打开漫画的方式顺序，不再用上面那个
    /// </summary>
    [DefaultValue(0)]
    int ReadMangaWayIndex { set; get; }

    /// <summary>
    /// 用户选择的打开漫画方式
    /// </summary>
    [DefaultValue("")]
    string UserSelectedReadMangaExePath { set; get; }

    /// <summary>
    /// 用户添加的exe路径拼接后字符串，路径由英文竖分割（不是中文竖）：|。因为config.net目前不支持写集合，所以用这个折中方法
    /// </summary>
    [DefaultValue("")]
    string PossibleExePaths { set; get; }
}

/// <summary>
///
/// </summary>
public static class ReadMangaWayStrings
{
    /// <summary>
    /// 内部ReadPage，
    /// </summary>
    public const string InternalReadPage = "InternalReadPage";

    /// <summary>
    /// 系统决定的关联打开方式
    /// </summary>
    public const string OSRelated = "OSRelated";

    /// <summary>
    /// 用户选定的exe
    /// </summary>
    public const string UserSelected = "UserSelected";
}

/// <summary>
/// 主题设置值，这个值对应ComboBox的索引值
/// </summary>
public static class ThemeValue
{
    /// <summary>
    /// 深色
    /// </summary>
    public const int Dark = 2;

    /// <summary>
    /// 浅色
    /// </summary>
    public const int Light = 1;

    /// <summary>
    /// 自动
    /// </summary>
    public const int Auto = 0;
}