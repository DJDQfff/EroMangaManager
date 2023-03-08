using System;
using System.IO;
using System.Linq;

using EroMangaManager.Models;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.Views.ContentDialogPages
{
    public sealed partial class RenameDialog : ContentDialog
    {
        private string str;

        public string NewDisplayName => textbox.Text;

        public RenameDialog (MangaBook mangaBook)
        {
            this.InitializeComponent();
            textbox.Text = mangaBook.FileDisplayName;
            oldname.Text = mangaBook.FileDisplayName;
            str = PrimaryButtonText;
        }

        private void textbox_TextChanged (object sender , TextChangedEventArgs e)
        {
            var bool1 = string.IsNullOrWhiteSpace(NewDisplayName);
            var invalidChars = Path.GetInvalidFileNameChars();
            var bool2 = NewDisplayName.Any(c => invalidChars.Contains(c));

            if (bool1)
            {
                // TODO 还要检查非法文件字符
                hinttextblock.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("StringResources").GetString("DontUseEmptyString");
                PrimaryButtonText = string.Empty;
            }
            else if (bool2)
            {
                hinttextblock.Text = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("StringResources").GetString("ContainInvalaidChar");
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