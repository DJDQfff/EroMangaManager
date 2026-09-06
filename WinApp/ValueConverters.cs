using UnoLibrary.Services;

namespace UnoLibrary.ValueConverters;

public partial class Base64ToImageConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string base64 && !string.IsNullOrEmpty(base64))
        {
            try
            {
                var bytes = System.Convert.FromBase64String(base64);
                MemoryStream stream = new(bytes);

                var bitmap = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();
                // Uno/WinUI 的 BitmapImage 支持从流同步设置源
                bitmap.SetSource(stream.AsRandomAccessStream());
                return bitmap;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Base64转图片失败: {ex.Message}");
                return null;
            }
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

internal partial class ControlVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            0 or null or true or MangasGroupUpdateState.Busy => Visibility.Visible,
            _ => (object)Visibility.Collapsed,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public partial class GetRandomSolidColorBrush : IValueConverter
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
        var brush = WindowsUIColorHelper.GetRandomSolidColorBrush();
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

public partial class ItemsConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string a)
        {
            var b = a.Split('|').SkipWhile(x => string.IsNullOrWhiteSpace(x));
            return b;
        }
        else
        {
            return null;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
