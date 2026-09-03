
namespace UnoApp;

public record Pages:IPages
{
   public NavigationItem[] MainPages { get; } = [
    
        new(typeof(NavigationPage),StringsEnum.Bookcase,"随机本子", new(Symbol.ViewAll)),
    ];

    public NavigationItem[] FooterPages { get; } =
    [

    ];


    public NavigationItem SettingPage { get; } = new (typeof(SettingPage),StringsEnum.Setting , "设置", new(Symbol.Setting));
}
