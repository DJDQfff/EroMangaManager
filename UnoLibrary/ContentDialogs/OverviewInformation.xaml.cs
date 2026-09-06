// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

using UnoLibrary.Services;

namespace WinApp.Views.ContentDialogPages;

/// <summary>
///
/// </summary>
public sealed partial class OverviewInformation : ContentDialog
{
    private Manga Manga { get; }

    /// <summary>
    ///
    /// </summary>
    public OverviewInformation(Manga manga, CoverHelper coverHelper)
    {
        InitializeComponent();
        Manga = manga;
    }
}
