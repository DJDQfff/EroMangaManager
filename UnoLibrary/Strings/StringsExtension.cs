using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.Resources;

namespace UnoLibrary.Strings;

///<summary> </summary>
public partial class StringsExtension : MarkupExtension
{
    public static ResourceLoader ResourceLoader { get; } = null!;

    ///<summary> </summary>
    public StringsEnum Uid { get; set; }

    // TODO 设置英语模式下，大小写模式
    //public UpperMode CharcaterMode { set; get; }
    static StringsExtension()
    {
        //弄成静态，免得外部频繁改
        ResourceLoader = ResourceLoader.GetForViewIndependentUse("/UnoLibrary/Resources");
    }

    ///<summary> </summary>
    protected override object ProvideValue() =>
        ResourceLoader.GetString(Uid.ToString()) ?? string.Empty;
}

public enum UpperMode
{
    /// <summary>
    /// 首字母大写
    /// </summary>
    FirstUpper,

    /// <summary>
    /// 全大写
    /// </summary>
    AllUpper,

    /// <summary>
    /// 全小写
    /// </summary>
    AllLower,
}
