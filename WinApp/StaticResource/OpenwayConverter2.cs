namespace EroMangaManager.WinUI3.StaticResource
{
    internal partial class OpenwayConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value switch
            {
                "InternalReadPage"
                or "OSRelated"
                    => ResourceLoader.GetForViewIndependentUse().GetString(value as string),
                string => Path.GetFileNameWithoutExtension(value as string),
                _ => null,
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}