using System.Collections.ObjectModel;

using EroMangaDB.Helper;

using MyLibrary.Standard20;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EroMangaManager.UWP.UserControls
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class TagsDisplayControl : UserControl
    {
        private ObservableCollection<string> tags = new ObservableCollection<string>();

        /// <summary>
        ///
        /// </summary>
        public string TagArray
        {
            set
            {
                var temp = value.SplitAndParser();
                tags.Clear();
                tags.AddRange(temp);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public TagsDisplayControl ()
        {
            this.InitializeComponent();
        }

        private void Border_Loaded (object sender , RoutedEventArgs e)
        {
            Border border = sender as Border;
            border.Background = MyLibrary.UWP.WindowsUIColorHelper.GetRandomSolidColorBrush();
        }
    }
}