namespace EroMangaManager.WinUI3.StaticResource
{
    /// <summary>
    /// 进度条可见性
    /// </summary>
    public partial class ProgressRingVisibility : IValueConverter
    {
        /// <summary>
        /// 类型转出
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                ReaderVM => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
        }

        /// <summary>
        /// 类型转回
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}