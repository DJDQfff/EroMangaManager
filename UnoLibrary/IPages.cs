




namespace UnoLibrary;

public interface IPages
{
    NavigationItem Find(StringsEnum uid);
    NavigationItem[] MainPages { get; }
    NavigationItem[] FooterPages { get; }
    NavigationItem SettingPage { get; }
}

