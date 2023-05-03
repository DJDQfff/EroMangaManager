using SharpCompress.Archives;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace EroMangaManager.UWP.StaticResource
{
    /// <summary>
    /// 用于ReadPage的数据模板选择器
    /// </summary>
    public class MyDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// 绑定到BitmapImage
        /// </summary>
        public DataTemplate OfBitmapImage { get; set; }

        /// <summary>
        /// 绑定到Entry
        /// </summary>
        public DataTemplate OfIArchiveEntry { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected override DataTemplate SelectTemplateCore(object item)
        {
            switch (item)
            {
                case BitmapImage _:
                    return OfBitmapImage;

                case IArchiveEntry _:
                    return OfIArchiveEntry;
            }
            return null;
        }
    }
}