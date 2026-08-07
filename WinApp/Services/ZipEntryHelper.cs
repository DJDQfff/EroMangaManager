using Microsoft.UI.Xaml.Media.Imaging;
using SharpCompress.Archives;
using SixLabors.ImageSharp;

namespace UnoLibrary.Services;

/// <summary>
/// 压缩文件帮助类
/// </summary>
public class ZipEntryHelper
{
    /// <summary>
    /// 获取bitmapimage
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    public async Task<BitmapImage> ToBitmapImage(IArchiveEntry entry)
    {
        // TODO 内部可以优化，试试不复制内存流直接读取
        BitmapImage bitmapImage = new();

        MemoryStream imageStream = new();

        entry.WriteTo(imageStream);
        imageStream.Position = 0;

        //TODO webp好像可以支持，没试过
        if (
            string.Equals(Path.GetExtension(entry.Key), ".webp", StringComparison.OrdinalIgnoreCase)
        )
        {
            var image = SixLabors.ImageSharp.Image.Load(imageStream);

            // TODO 这个MemoryStream总是返回null，不知道为什么
            imageStream.Dispose();
            imageStream = new MemoryStream();

            image.SaveAsPng(imageStream);
            imageStream.Position = 0;
        }
        var randomAccessStream = imageStream.AsRandomAccessStream();

        randomAccessStream.Seek(0); //记得偏移量归零，

        await bitmapImage.SetSourceAsync(randomAccessStream);

        return bitmapImage;
    }
}
