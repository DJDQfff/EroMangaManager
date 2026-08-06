// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Database;

namespace UnoLibrary.ContentDialogPages;

public sealed partial class TagCategorySelect : ContentDialog
{
    readonly DatabaseController databaseController;

    public TagCategorySelect(MainWindow mainWindow, DatabaseController _databaseController)
    {
        InitializeComponent();
        databaseController = _databaseController;
        XamlRoot = mainWindow.Content!.XamlRoot;
    }

    public string? CategoryName => combobox.SelectedItem as string;

    private void Combobox_Loaded(object sender, RoutedEventArgs e)
    {
        var category = databaseController.TagCategory_Query();
        if (category != null)
        {
            combobox.ItemsSource = category;
        }
    }
}
