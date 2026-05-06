using SixLabors.ImageSharp;

namespace EroMangaManager.WinUI3.Helpers
{
    /// <summary>
    /// 压缩文件帮助类
    /// </summary>
    public static class ZipEntryHelper
    {
        /// <summary>
        /// 获取bitmapimage
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static async Task<BitmapImage> ToBitmapImage(this IArchiveEntry entry)
        {
            // TODO 内部可以优化，试试不复制内存流直接读取
            var bitmapImage = new BitmapImage();

            var imageStream = new MemoryStream();

            entry.WriteTo(imageStream);
            imageStream.Position = 0;

            IRandomAccessStream randomAccessStream;

            //TODO webp好像可以支持，没试过
            if (Path.GetExtension(entry.Key).ToLower() == ".webp")
            {
                var image = SixLabors.ImageSharp.Image.Load(imageStream);

                // TODO 这个MemoryStream总是返回null，不知道为什么
                imageStream.Dispose();
                imageStream = new MemoryStream();

                image.SaveAsPng(imageStream);
                imageStream.Position = 0;
            }
            randomAccessStream = imageStream.AsRandomAccessStream();

            randomAccessStream.Seek(0); //记得偏移量归零，

            await bitmapImage.SetSourceAsync(randomAccessStream);

            return bitmapImage;
        }
    }
}