using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238
// 上介绍了“空白页”项模板

namespace EroMangaManager.Views.SettingSubPages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class FiltedImagesPage : Page
    {
        private readonly ObservableCollection<ImageItem> items = new ObservableCollection<ImageItem>();

        public FiltedImagesPage ()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ImageItem.GetsAsync(items);
        }

        private async void Button_Click (object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            button.IsEnabled = false;

            IList<object> listobject = GridView.SelectedItems;
            List<string> hashlist = new List<string>();
            for (int i = listobject.Count - 1; i >= 0; i--)              // 还是老问题，遍历删除不能使用foreach或正序for循环要用倒序for
            {
                ImageItem imageItem = listobject[i] as ImageItem;
                items.Remove(imageItem);                // items和listobjects本质上是同一个集合，只不过以不同类型打开
                string hash = imageItem.storageFile.DisplayName;
                hashlist.Add(hash);
                await imageItem.storageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            }
            string[] vs = hashlist.ToArray();
            await EroMangaManager.Models.HashManager.Remove(vs);
            button.IsEnabled = true;
        }
    }
}