namespace UnoApp;

internal class ShowErrorDebug
{
    public static async void ShowErrorContentDialog(string message)
    {
#if DEBUG
        await new ContentDialog
        {
            Title = "错误",
            Content = message,
            CloseButtonText = "确定",
        }.ShowAsync();
#else
        System.Diagnostics.Debug.WriteLine(message);
#endif
    }
}
