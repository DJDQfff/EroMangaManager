
namespace UnoApp;

public partial class Base64ToImageConverter : IValueConverter
{
    public object? Convert (object value , Type targetType , object parameter , string language)
    {
        if (value is string base64 && !string.IsNullOrEmpty(base64))
        {
            try
            {
                var bytes = System.Convert.FromBase64String(base64);
                MemoryStream stream = new(bytes);
                BitmapImage bitmap = new ();
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

    public object? ConvertBack (object value , Type targetType , object parameter , string language)
    {
        throw new NotImplementedException();
    }
}

