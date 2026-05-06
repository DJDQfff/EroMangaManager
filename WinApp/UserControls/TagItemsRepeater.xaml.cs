// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.UserControls;

public sealed partial class TagItemsRepeater : UserControl
{
    public Manga Manga
    {
        get => DataContext as Manga;
        set
        {
            DataContext = value;
        }
    }

    public TagItemsRepeater()
    {
        InitializeComponent();
        DataContextChanged += (a, b) => this.Bindings.Update();
    }
}