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
                    var container = TagGroup.ContainerFromItem(item) as ListViewItem;
                    var root = container.ContentTemplateRoot as StackPanel;
                    var textBlock = root.FindName("tag") as TextBlock;
                    var tag = textBlock.Text;
                    var kohc = root.FindName("combo") as ComboBox;
                    var k = (kohc.SelectedItem as string) ?? "[]";

                    stringBuilder.Append(k[0]);
                    stringBuilder.Append(tag);
                    stringBuilder.Append(k[1]);
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

        private void ResultName_Loaded(object sender, RoutedEventArgs e)
        {
            ResultName.Text = NewDisplayName;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResultName.Text = NewDisplayName;
        }
    }

    internal class TagWork
    {
        public string Tag { get; set; }
        public string Left { get; set; }
    }
}