using System;

using Windows.UI.Xaml;

namespace EroMangaManager.UWP.ValueConverters
{
    internal class ControlVisibility : Windows.UI.Xaml.Data.IValueConverter
    {
        public object Convert (object value , Type targetType , object parameter , string language)
        {
            if (value is int count)
            {
                return VisibilityFromCount(count);
            }
            else if (value is bool b)
            {
                return VIsibilityFromBool(b);
            }
            else
            {
                return VisibilityFromNull(value);
            }
        }

        private Visibility VIsibilityFromBool (bool b)
        {
            if (b is true)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        private Visibility VisibilityFromNull (object o)
        {
            if (o is null)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        private Visibility VisibilityFromCount (int count)
        {
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