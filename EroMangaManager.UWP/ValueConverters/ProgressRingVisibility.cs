using System;

using EroMangaManager.UWP.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace EroMangaManager.UWP.ValueConverters
{
    /// <summary>
    /// 进度条可见性
    /// </summary>
    public class ProgressRingVisibility : IValueConverter
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
            ReaderVM reader = value as ReaderVM;
            if (reader is null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
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