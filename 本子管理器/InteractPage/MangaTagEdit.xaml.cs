﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using EroMangaManager.Models;

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

using static EroMangaTagDatabase.Controller;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.InteractPage
{
    public sealed partial class MangaTagEdit : ContentDialog
    {
        private MangaBook mangaBook;
        private ObservableCollection<string> taglist;
        public List<string> comboxitemlist { set; get; }

        public MangaTagEdit (MangaBook _mangaBook)
        {
            this.InitializeComponent();
            mangaBook = _mangaBook;
            var tags = _mangaBook.ReadingInfo.TagPieces.Split('\r');
            taglist = new ObservableCollection<string>(tags);

            comboxitemlist = DatabaseController.TagKeywords_QueryAll().Keys.ToList();
            comboxitemlist.Add("忽略此标签");
        }

        private async void ContentDialog_PrimaryButtonClick (ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var tags = List.Items;
            Dictionary<string, string> moveWork = new Dictionary<string, string>();
            foreach (var tag in tags)
            {
                var keyword = tag as string;
                var container = List.ContainerFromItem(tag) as ListViewItem;
                var rootpanel = container.ContentTemplateRoot as RelativePanel;
                var combobox = rootpanel.FindName("ComboBox") as ComboBox;

                if (combobox.SelectedIndex == comboxitemlist.Count - 1)             // 最后一项要忽略
                    continue;

                var tagname = combobox.SelectedItem as string;

                moveWork.Add(keyword, tagname);
            }
            await DatabaseController.TagKeywords_MoveMulti(moveWork);
        }

        /// <summary>
        /// 实例化的时候会执行，之后每次切换都会执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0)                  // 初始化
                return;

            ComboBox comboBox = sender as ComboBox;
            var index = comboBox.SelectedIndex;

            if (index != (comboxitemlist.Count - 1))
            {
                string tagname = comboxitemlist[index];

                string keyword = comboBox.DataContext as string;

                await DatabaseController.TagKeywords_AppendKeywordSingle(tagname, keyword);
            }
        }
    }

    public class IndexConverter : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, string language)
        {
            string str = value as string;
            if (str is null)
                return null;

            Dictionary<string, string[]> keyValuePairs = DatabaseController.TagKeywords_QueryAll();

            int i = 0;
            foreach (var pair in keyValuePairs)
            {
                if (pair.Value.Contains(str))
                {
                    break;
                }
                i++;
            }
            return i;
        }

        public object ConvertBack (object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}