namespace EroMangaManager.WinUI3.StaticResource;

internal partial class DisplayUnits : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var amount = (int)value;
        var unit = parameter switch
        {
            "0" => ResourceLoader.GetForViewIndependentUse().GetString("Chapters"),
            "1" => ResourceLoader.GetForViewIndependentUse().GetString("Images"),
            _ => ""
        };
        return amount + unit;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
};