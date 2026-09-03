namespace UnoLibrary;

public class WinUISetting : ISettingFilePath
{
    public string IniPath =>
        Path.Combine(ApplicationData.Current.LocalFolder.Path, "AppConfig.ini");
}
