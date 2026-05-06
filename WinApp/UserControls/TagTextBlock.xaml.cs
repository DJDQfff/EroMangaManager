using Windows.ApplicationModel.DataTransfer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.UserControls;

public sealed partial class TagTextBlock : UserControl
{
    public string Text
    {
        set
        {
            textblock.Text = value;
        }
    }

    public TagTextBlock()
    {
        InitializeComponent();
    }

    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        DataPackage dataPackage = new();
        dataPackage.SetText(textblock.Text);
        dataPackage.RequestedOperation = DataPackageOperation.Copy;

        Clipboard.SetContent(dataPackage);
    }

    private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
    {
        MainPage.Current.MainFrame.Navigate(typeof(GlobalSearchPage), new string[] { textblock.Text });
    }
}