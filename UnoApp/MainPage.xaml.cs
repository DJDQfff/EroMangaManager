namespace UnoApp;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (await App.MangaClient.CheckConnectionAsync())
        {
            mainFrame.Navigate(typeof(NavigationPage));
        }
        else
        {
            mainFrame.Navigate(typeof(ConnectPage));
        }
    }
}
