namespace EroMangaManager.WinUI3.Strings;

///<summary> </summary>
public class StringsExtension : MarkupExtension
{
    ///<summary> </summary>
    public StringsEnum Uid { get; set; }

    // TODO 设置英语模式下，大小写模式
    //public UpperMode CharcaterMode { set; get; }

    ///<summary> </summary>
    protected override object ProvideValue()
    {
        return ResourceLoader.GetForViewIndependentUse().GetString(Uid.ToString());
    }
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
    AllLower
}