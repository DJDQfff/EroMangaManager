namespace EroMangaManager.WinUI3.StaticResource;

internal partial class MangasGroupDisplayPath : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string path)
        {
            var a = App.Current.GlobalViewModel.MangaFolders.SingleOrDefault(
                x => x.FolderPath == path
            );
            return a;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is MangasGroup mangas)
        {
            return mangas.FolderPath;
        }
        return null;
    }
}
