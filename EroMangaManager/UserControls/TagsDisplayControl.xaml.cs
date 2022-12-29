using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MyStandard20Library;
using MyUWPLibrary;

using EroMangaDB.Helper;
//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EroMangaManager.UserControls
{
    public sealed partial class TagsDisplayControl : UserControl
    {
        ObservableCollection<string> tags=new ObservableCollection<string>();
        public string TagArray
        {
            set
            {
                var temp = value.SplitAndParser();
                tags.Clear();
                tags.AddRange(temp);
            }
        }
        public TagsDisplayControl ()
        {
            this.InitializeComponent();

        }


        private void Border_Loaded (object sender , RoutedEventArgs e)
        {
            Border border = sender as Border;
            border.Background = MyUWPLibrary.WindowsUIColorHelper.GetRandomSolidColorBrush();
        }
    }
}
