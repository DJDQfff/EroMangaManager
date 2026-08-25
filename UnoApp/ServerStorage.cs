namespace UnoApp;

public  class ServerStorage
{
    private readonly string Key = "last_server_url";

    public  string? LoadLastServer()
    {
        if (ApplicationData.Current.LocalSettings.Values.TryGetValue(Key, out object? value))
        {
            var s = value?.ToString();
            if (!string.IsNullOrEmpty(s))
                return s;
        }
        return null;
    }

    public  void SaveServer(string url)
    {
        ApplicationData.Current.LocalSettings.Values[Key] = url.TrimEnd('/');
    }
}
