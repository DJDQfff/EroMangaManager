using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.UI.Xaml.Data;
using EroMangaManager.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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