using Windows.UI.Xaml.Controls;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace EroMangaManager.UserControls
{
    /// <summary>
    /// 一堆Tag
    /// </summary>
    public sealed partial class TagInfo : UserControl
    {
        /// <summary>
        /// 标签名
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// 标签值
        /// </summary>
        public string TagValue { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        public TagInfo ()
        {
            this.InitializeComponent();
        }
    }
}