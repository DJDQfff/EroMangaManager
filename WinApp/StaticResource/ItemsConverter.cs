namespace EroMangaManager.WinUI3.StaticResource
{
    internal partial class ItemsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
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
}