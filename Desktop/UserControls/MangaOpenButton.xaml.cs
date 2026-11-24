// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Desktop.UserControls;

public sealed partial class MangaOpenButton : UserControl
{
    private readonly SettingViewModel viewModel = App.Services.GetRequiredService<SettingViewModel>();

    public MangaOpenButton()
    {
        this.InitializeComponent();
    }
}