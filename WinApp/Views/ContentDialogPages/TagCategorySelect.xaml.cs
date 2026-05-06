// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.Views.ContentDialogPages;

public sealed partial class TagCategorySelect : ContentDialog
{
    public TagCategorySelect()
    {
        InitializeComponent();
    }

    public string CategoryName => combobox.SelectedItem as string;

    private void Combobox_Loaded(object sender, RoutedEventArgs e)
    {
        var category = DatabaseController.TagCategory_Query();
        if (category != null)
        { combobox.ItemsSource = category; }
    }
}