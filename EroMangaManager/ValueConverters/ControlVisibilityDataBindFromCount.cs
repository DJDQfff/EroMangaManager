using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;

namespace EroMangaManager.ValueConverters
{
    internal class ControlVisibilityDataBindFromCount:Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert (object value , Type targetType , object parameter , string language)
        {
           var count=(int)value;
           
            if (count == 0)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack (object value , Type targetType , object parameter , string language)
        {
            throw new NotImplementedException();
        }
    }
}
