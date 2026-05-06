// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.Views.ContentDialogPages;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class RenameCategoryName : ContentDialog
{
    public RenameCategoryName()
    {
        this.InitializeComponent();
    }

    public string Newname => RenameBox.Text;

    private bool CheckNameValid(string name)
    {
        return !string.IsNullOrWhiteSpace(RenameBox.Text);
    }

    private void RenameBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(RenameBox.Text);
    }
}