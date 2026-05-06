// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace EroMangaManager.WinUI3.UserControls.MangaBasicInfo
{
    public sealed partial class SizeImagesChaptersAmount : UserControl
    {
        public Manga Manga { get => DataContext as Manga; set => DataContext = value; }

        public SizeImagesChaptersAmount()
        {
            InitializeComponent();
            DataContextChanged += (a, b) => this.Bindings.Update();
        }
    }
}