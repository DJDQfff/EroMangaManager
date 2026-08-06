//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace WinApp.UserControls;

/// <summary>
/// 一堆Tag
/// </summary>
public sealed partial class TagInfo : UserControl
{
    // TODO 可以考虑将 TagName 和 TagValue 改为依赖属性，以便在 XAML 中绑定和使用

    /// <summary>
    /// 标签名
    /// </summary>
    public string? TagName { get; set; }

    /// <summary>
    /// 标签值
    /// </summary>
    public string? TagValue { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    public TagInfo()
    {
        this.InitializeComponent();
    }
}
