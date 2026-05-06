namespace EroMangaManager.WinUI3.OldLibrary;

/// <summary>
/// WindowsColor帮助类
/// </summary>
public class WindowsUIColorHelper
{
    /// <summary>
    /// 获取随机颜色SolidColorBrush
    /// </summary>
    /// <returns></returns>
    public static SolidColorBrush GetRandomSolidColorBrush()
    {
        //Type type = typeof(Windows.UI.Colors);
        //var a = type.GetProperties();
        //Random random = new Random();
        //var b = random.Next(0, a.Length);
        //var c = a[b];
        //var d = (Windows.UI.Color) c.GetValue(b);

        var random2 = new Random();

        byte[] ragb = new byte[4];

        random2.NextBytes(ragb);

        //TODO 看看uno是怎么改的
        var color = Windows.UI.Color.FromArgb(ragb[0], ragb[1], ragb[2], ragb[3]);
        var solidColorBrush = new SolidColorBrush(color);

        return solidColorBrush;
    }
}