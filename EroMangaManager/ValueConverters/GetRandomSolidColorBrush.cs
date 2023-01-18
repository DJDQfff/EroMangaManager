using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EroMangaManager.ValueConverters
{
    public class GetRandomSolidColorBrush:Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert (object value , Type targetType , object parameter , string language)
        {
            var brush = MyUWPLibrary.WindowsUIColorHelper.GetRandomSolidColorBrush();
            return brush;
        }

        public object ConvertBack (object value , Type targetType , object parameter , string language)
        {
            throw new NotImplementedException();
        }
    }
}
