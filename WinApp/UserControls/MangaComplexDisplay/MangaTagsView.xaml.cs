// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.UserControls.MangaComplexDisplay;

public sealed partial class MangaTagsView : UserControl
{

    public Manga Manga
    {
        get => DataContext as Manga;
        set
        {
            DataContext = value;
        }
    }

    public MangaTagsView()
    {
        InitializeComponent();
        DataContextChanged += (a, b) => this.Bindings.Update();

    }
}