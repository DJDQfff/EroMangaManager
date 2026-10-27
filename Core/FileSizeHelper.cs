namespace Core;
public static class FileSizeHelper
{
    private static readonly string[] BinaryUnits = ["B" , "KiB" , "MiB" , "GiB" , "TiB" , "PiB"];
    private static readonly string[] DecimalUnits = ["B" , "KB" , "MB" , "GB" , "TB" , "PB"];

    /// <summary>
    /// 格式化文件大小（IEC 二进制标准，1024 进制），例如：KiB, MiB， GiB
    /// </summary>
    public static string FormatFileSizeBinary (long bytes)
    {
        if (bytes <= 0)
            return "0 B";

        int unitIndex = 0;
        double size = bytes;

        while (size >= 1024 && unitIndex < BinaryUnits.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:F1} {BinaryUnits[unitIndex]}";
    }

    /// <summary>
    /// 格式化文件大小（SI 十进制标准，1000 进制），单位为 KB、MB、GB 等
    /// </summary>
    public static string FormatFileSizeDecimal (long bytes)
    {
        if (bytes <= 0)
            return "0 B";

        int unitIndex = 0;
        double size = bytes;

        while (size >= 1000 && unitIndex < DecimalUnits.Length - 1)
        {
            size /= 1000;
            unitIndex++;
        }

        return $"{size:F1} {DecimalUnits[unitIndex]}";
    }
}