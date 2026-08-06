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

public partial class FileSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        //版权声明：本文为博主原创文章，遵循 CC 4.0 BY - SA 版权协议，转载请附上原文出处链接和本声明。
        //原文链接：https://blog.csdn.net/qq395537505/article/details/51025812

        var size = System.Convert.ToDouble(value); //((double)value);
        string[] units = ["B", "KB", "MB", "GB", "TB", "PB"];
        double mod = 1024;
        int i = 0;
        while (size >= mod)
        {
            size /= (long)mod;
            i++;
        }
        return Math.Round((double)size) + " " + units[i];

        //菜鸡的我写的
        //var kb = size >> 10;
        //var mb = kb >> 10;
        //if (mb > 1000)
        //{
        //    var gb = mb >> 10;
        //    return gb + " GB";
        //}
        //else
        //{
        //    if (mb == 0)
        //    {
        //        return "<1 MB";
        //    }
        //    return mb + " MB";
        //}
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
};

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
