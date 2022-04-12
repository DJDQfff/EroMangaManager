using System;

using EroMangaManager.Models;
using EroMangaManager.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace EroMangaManager.ValueConverters
{
    public class ProgressRingVisibility : IValueConverter
    {
        public object Convert (object value, Type targetType, object parameter, string language)
        {
            Reader reader = value as Reader;
            if (reader is null)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack (object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}