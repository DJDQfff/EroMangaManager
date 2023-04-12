using System;
using System.IO;
using System.Linq;

using EroMangaManager.Core.Models;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.UWP.Views.ContentDialogPages
{
    /// <summary>
    /// 重命名对话框
    /// </summary>
    public sealed partial class RenameDialog : ContentDialog
    {
        private readonly string str;

        /// <summary>
        /// 选择的新名字
        /// </summary>
        public string NewDisplayName => textbox.Text;

        /// <summary>
        ///
        /// </summary>
        /// <param name="mangaBook"></param>
        /// <param name="suggestedname"></param>
        public RenameDialog(MangaBook mangaBook, string suggestedname)
        {
            this.InitializeComponent();
            textbox.Text = suggestedname ?? mangaBook.FileDisplayName;
            oldname.Text = mangaBook.FileDisplayName;
            str = PrimaryButtonText;
        }

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var bool1 = string.IsNullOrWhiteSpace(NewDisplayName);
            var invalidChars = Path.GetInvalidFileNameChars();
            var bool2 = NewDisplayName.Any(c => invalidChars.Contains(c));
            if (bool1)
            {
                // 检查文件名是非为空
                hinttextblock.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("Strings").GetString("DontUseEmptyString");
                PrimaryButtonText = string.Empty;
            }
            else if (bool2)
            {
                // 检查文件是否含有非法字符
                hinttextblock.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("Strings").GetString("ContainInvalaidChar");
                PrimaryButtonText = string.Empty;
            }
            else
            {
                hinttextblock.Text = string.Empty;
                PrimaryButtonText = str;
            }
        }
    }
}