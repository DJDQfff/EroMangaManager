// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.WinUI3.Views.ContentDialogPages
{
    /// <summary>
    /// 单个漫画文件的重命名对话框
    /// </summary>
    public sealed partial class RenameDialog : ContentDialog
    {
        /// <summary>
        /// 新名称
        /// </summary>
        public string NewDisplayName => renamecontrol.NewDisplayName;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mangaBook"></param>
        /// <param name="suggestedname"></param>
        public RenameDialog(Manga mangaBook, string suggestedname) : base()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mangaBook"></param>
        public RenameDialog(Manga mangaBook)
        {
            this.InitializeComponent();
            this.renamecontrol.Manga = mangaBook;
        }
    }
}