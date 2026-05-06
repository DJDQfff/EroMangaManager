namespace EroMangaManager.WinUI3.StaticResource;

internal class FileSizeConverter : IValueConverter
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
        return Math.Round((double)size) + units[i];

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