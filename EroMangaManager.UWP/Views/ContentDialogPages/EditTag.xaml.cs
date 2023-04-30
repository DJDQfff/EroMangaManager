using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using EroMangaDB.Migrations;

using EroMangaManager.Core.Models;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using EroMangaDB.Helper;
using System.Text;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.UWP.Views.ContentDialogPages
{
    public sealed partial class EditTag : ContentDialog
    {
        // TODO  不能使用GridView，太难看
        public MangaBook MangaBook { set; get; }

        public string NewDisplayName
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();

                var items = TagGroup.ItemsSource as List<TagWork>;
                foreach (var item in items)
                {
                    TagWork tagWork = item as TagWork;
                    stringBuilder.Append(tagWork.Left[0]);
                    stringBuilder.Append(tagWork.Tag);
                    stringBuilder.Append(tagWork.Left[1]);
                }
                return stringBuilder.ToString();
            }
        }

        public EditTag(MangaBook mangaBook)
        {
            this.InitializeComponent();

            MangaBook = mangaBook;
            Initial();
        }

        public void Initial()
        {
            string displayname = MangaBook.FileDisplayName;
            var a = displayname.SplitAndParser();

            var list = new List<TagWork>();
            for (int index = 0; index < a.Count; index++)
            {
                var currentTag = a[index];
                TagWork tagWork;
                tagWork = new TagWork() { Tag = currentTag, Left = "[]" };
                list.Add(tagWork);
            }
            TagGroup.ItemsSource = list;
        }
    }

    internal class TagWork
    {
        public string Tag { get; set; }
        public string Left { get; set; }
    }
}