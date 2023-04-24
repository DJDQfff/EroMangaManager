using System;

namespace EroMangaManager.UWP.StaticResource
{
    /// <summary>
    /// 返回随机SolidSolorBrtush
    /// </summary>
    public class GetRandomSolidColorBrush : Windows.UI.Xaml.Data.IValueConverter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = MyLibrary.UWP.WindowsUIColorHelper.GetRandomSolidColorBrush();
            return brush;
        }

        /// <summary>
        ///
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