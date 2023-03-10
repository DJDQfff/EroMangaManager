using EroMangaDB.Entities;

using EroMangaManager.UWP.Models;
using EroMangaManager.UWP.UserControls;

using Windows.UI.Xaml.Controls;

using static EroMangaDB.BasicController;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“内容对话框”项模板

namespace EroMangaManager.UWP.Views.ContentDialogPages
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class MangaTagDetail : ContentDialog
    {
        private ReadingInfo readingInfo;

        /// <summary>
        /// Tag细节对话框
        /// </summary>
        /// <param name="_mangaBook"></param>
        public MangaTagDetail (MangaBook _mangaBook)
        {
            this.InitializeComponent();
            var tags = readingInfo.TagsAddedByUser.Split('\r');
            var list = DatabaseController.MatchTag(tags);
            foreach (var l in list)
            {
                TagInfo tag = new TagInfo()
                {
                    TagName = l.Value ?? "未知标签" ,
                    TagValue = l.Key
                };
                Stack.Children.Add(tag);
            }
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
                    return "中文";

                case "Japanese":
                    return "日语";

                case string str:
                    return str;

                default:
                    return "未知";
            }
        }
    }
}