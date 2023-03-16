using EroMangaManager.UWP.UserControls;

using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace EroMangaManager.UWP.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TestPage : Page
    {
        /// <summary>
        /// 测试用页面
        /// </summary>
        public TestPage()
        {
            this.InitializeComponent();

            TagsDisplayControl tagsDisplayControl = new TagsDisplayControl()
            {
            };

            var grid = this.Content as Grid;
            grid.Children.Add(tagsDisplayControl);
        }
    }
}