namespace EroMangaManager.WinUI3.Commands;

internal class UICommands
{
    public static UICommands Instance { get; set; }
    public StandardUICommand OverviewInformation = new();
    public StandardUICommand SearchSimilar = new();

    public static void Initial()
    {
        Instance ??= new();

        Instance.OverviewInformation.ExecuteRequested += async (sender, args) =>
        {
            switch (args.Parameter)
            {
                case Manga manga:
                    var dialog = new OverviewInformation(manga)
                    {
                        XamlRoot = App.Current.MainWindow.Content.XamlRoot
                    };
                    _ = await dialog.ShowAsync();
                    break;
            }
        };
        Instance.OverviewInformation.IconSource = new SymbolIconSource()
        {
            Symbol = Symbol.View
        };

        Instance.SearchSimilar.ExecuteRequested += (sender, args) =>
        {
            MainPage.Current.MainFrame.Navigate(typeof(GlobalSearchPage), args.Parameter);
        };
        Instance.SearchSimilar.IconSource = new SymbolIconSource() { Symbol = Symbol.Find };
    }
}