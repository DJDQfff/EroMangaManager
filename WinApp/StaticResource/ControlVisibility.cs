namespace EroMangaManager.WinUI3.StaticResource;

internal class ControlVisibility : IValueConverter
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