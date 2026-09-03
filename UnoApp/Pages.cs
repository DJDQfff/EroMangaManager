
namespace UnoApp;

public record Pages:IPages
{
    public NavigationItem Find(StringsEnum uid)
    {
        var item = MainPages.Concat(FooterPages).Concat([SettingPage]).SingleOrDefault(x => x.Uid == uid);
        if (item is null)
        {
            throw new Exception($"未找到页面 {uid}");
        }
        return item;
    }
   public NavigationItem[] MainPages { get; } = [
    
        new(typeof(NavigationPage),StringsEnum.Bookcase,"随机本子", new(Symbol.ViewAll)),
    ];

    public NavigationItem[] FooterPages { get; } =
    [

    ];


    public NavigationItem SettingPage { get; } = new (typeof(SettingPage),StringsEnum.Setting , "设置", new(Symbol.Setting));
}
