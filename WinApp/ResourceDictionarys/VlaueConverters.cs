namespace WinApp.ValueConverters;

internal partial class LocalizationWordsConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            "InternalReadPage" or "OSRelated" => StringsExtension.ResourceLoader.GetString(
                value as string
            ),
            string => value as string,
            _ => null,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

internal partial class MangasGroupDisplayPath : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string path)
        {
            var a = App
                .Services.GetRequiredService<ObservableCollectionVM>()
                .MangasGroups.SingleOrDefault(x => x.FolderPath == path);
            return a;
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is MangasGroup mangas)
        {
            return mangas.FolderPath;
        }
        return null;
    }
}

internal partial class OpenwayConverter2 : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, string language)
    {
        return value switch
        {
            "InternalReadPage" or "OSRelated" => StringsExtension.ResourceLoader.GetString(
                value as string
            ),
            string str => Path.GetFileNameWithoutExtension(str),
            _ => null,
        };
    }

    public object? ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
