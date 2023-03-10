using EroMangaManager.UWP.Models;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“内容对话框”项模板

namespace EroMangaManager.UWP.Views.ContentDialogPages
{
    /// <summary>
    /// 确认删除对话框
    /// </summary>
    public sealed partial class ConfirmDeleteMangaFile : ContentDialog
    {
        private MangaBook manga;

        /// <summary>
        /// 确认删除对话框
        /// </summary>
        /// <param name="_mangaBook"></param>
        public ConfirmDeleteMangaFile (MangaBook _mangaBook)
        {
            this.InitializeComponent();
            manga = _mangaBook;
        }
    }
}