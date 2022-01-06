using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EroMangaManager.Models;
using EroMangaManager.UserControls;
using EroMangaManager.Database.Entities;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“内容对话框”项模板

namespace EroMangaManager.InteractPage
{
    public sealed partial class MangaTagDetail : ContentDialog
    {
        private MangaTag mangaTag;

        public MangaTagDetail (MangaBook _mangaBook)
        {
            this.InitializeComponent();

            mangaTag = _mangaBook.TagInfo;
        }

        private void ContentDialog_PrimaryButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private string ConvertTagValue (object tagvalue)
        {
            switch (tagvalue)
            {
                case true:
                    return "是";

                case false:
                    return "否";

                case "English":
                    return "英语";

                case "Chinese":
                    return "汉语";

                case "Japanese":
                    return "日语";

                default:
                    return "未知";
            }
        }
    }
}