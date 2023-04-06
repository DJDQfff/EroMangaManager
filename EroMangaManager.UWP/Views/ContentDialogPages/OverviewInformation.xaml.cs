using EroMangaManager.Core.Models;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.UWP.Views.ContentDialogPages
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class OverviewInformation : ContentDialog
    {
        private readonly MangaBook MangaBook;

        /// <summary>
        ///
        /// </summary>
        public OverviewInformation(MangaBook manga)
        {
            this.InitializeComponent();
            MangaBook = manga;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}