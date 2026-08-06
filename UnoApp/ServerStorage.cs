namespace UnoApp;

public static class ServerStorage
{
    private const string Key = "last_server_url";

    public static string? LoadLastServer()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(Key, out object? value))
        {
            var s = value?.ToString();
            if (!string.IsNullOrEmpty(s))
                return s;
        }
        return null;
    }

    public static void SaveServer(string url)
    {
        ApplicationData.Current.LocalSettings.Values[Key] = url.TrimEnd('/');
    }
}
