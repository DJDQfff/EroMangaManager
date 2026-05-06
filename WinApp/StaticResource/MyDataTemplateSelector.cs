namespace EroMangaManager.WinUI3.StaticResource
{
    /// <summary>
    /// 用于ReadPage的数据模板选择器
    /// </summary>
    public partial class MyDataTemplateSelector : DataTemplateSelector
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
            return item switch
            {
                BitmapImage _ => OfBitmapImage,
                IArchiveEntry _ => OfIArchiveEntry,
                _ => null,
            };
        }
    }
}