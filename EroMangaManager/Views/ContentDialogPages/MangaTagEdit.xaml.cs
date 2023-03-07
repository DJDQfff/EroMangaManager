using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using EroMangaManager.Models;

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

using static EroMangaDB.BasicController;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“内容对话框”项模板

namespace EroMangaManager.Views.ContentDialogPages
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class MangaTagEdit : ContentDialog
    {
        private MangaBook mangaBook;
        private ObservableCollection<string> taglist;

        /// <summary>
        /// 组合框细节
        /// </summary>
        public List<string> comboxitemlist { set; get; }

        /// <summary>
        /// tag编辑对话框
        /// </summary>
        /// <param name="_mangaBook"></param>
        public MangaTagEdit (MangaBook _mangaBook)
        {
            this.InitializeComponent();
            mangaBook = _mangaBook;

            comboxitemlist = DatabaseController.TagCategory_QueryAll().Keys.ToList();
            comboxitemlist.Add("忽略此标签");
        }

        private async void ContentDialog_PrimaryButtonClick (ContentDialog sender , ContentDialogButtonClickEventArgs args)
        {
            var tags = List.Items;
            Dictionary<string , string> moveWork = new Dictionary<string , string>();
            foreach (var tag in tags)
            {
                var keyword = tag as string;
                var container = List.ContainerFromItem(tag) as ListViewItem;
                var rootpanel = container.ContentTemplateRoot as RelativePanel;
                var combobox = rootpanel.FindName("ComboBox") as ComboBox;

                if (combobox.SelectedIndex == comboxitemlist.Count - 1)             // 最后一项要忽略
                    continue;

                var tagname = combobox.SelectedItem as string;

                moveWork.Add(keyword , tagname);
            }
            await DatabaseController.TagCategory_MoveMulti(moveWork);
        }

        /// <summary>
        /// 实例化的时候会执行，之后每次切换都会执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ComboBox_SelectionChanged (object sender , SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count == 0)                  // 初始化
                return;

            ComboBox comboBox = sender as ComboBox;
            var index = comboBox.SelectedIndex;

            if (index != (comboxitemlist.Count - 1))
            {
                string tagname = comboxitemlist[index];

                string keyword = comboBox.DataContext as string;

                await DatabaseController.TagCategory_AppendKeywordSingle(tagname , keyword);
            }
        }
    }

    /// <summary>
    /// 索引转换
    /// </summary>
    public class IndexConverter : IValueConverter
    {
        /// <summary>
        /// 转出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert (object value , Type targetType , object parameter , string language)
        {
            string str = value as string;
            if (str is null)
                return null;

            Dictionary<string , string[]> keyValuePairs = DatabaseController.TagCategory_QueryAll();

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

        /// <summary>
        /// 转回
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack (object value , Type targetType , object parameter , string language)
        {
            throw new NotImplementedException();
        }
    }
}