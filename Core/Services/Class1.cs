using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Action;
using iText.Kernel.Pdf.Navigation;
using iText.Layout;
using iText.Layout.Element;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Core.Services;

public partial class Exporter
{
    /// <summary>
    /// 导出为PDF，使用iText7库，存在bug，暂时弃用
    /// </summary>
    /// <param name="manga"></param>
    /// <param name="fileName"></param>
    [Obsolete("有bug")]
    public void Export_iText(Manga manga, string fileName)
    {
        // 1. 创建 iText7 的 PDF 写入器和文档
        using var writestream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
        using var pdfWriter = new PdfWriter(writestream);
        using var pdfDocument = new PdfDocument(pdfWriter);
        using var document = new Document(pdfDocument);

        // 设置页边距为 0，让图片铺满页面
        document.SetMargins(0, 0, 0, 0);

        foreach (var chapter in manga.Chapters)
        {
            int startPageCount = pdfDocument.GetNumberOfPages();

            foreach (var stream in mangaStreamProviderm.GetStreams(chapter))
            {
                using (stream)
                {
                    try
                    {
                        if (stream.CanSeek)
                            stream.Position = 0;
                        using var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        var format = SixLabors.ImageSharp.Image.DetectFormat(memoryStream);
                        memoryStream.Position = 0;

                        // 处理 WebP 转 JPEG
                        if (format?.Name == "Webp")
                        {
                            using var image = SixLabors.ImageSharp.Image.Load(memoryStream);
                            memoryStream.SetLength(0);
                            image.Save(memoryStream, new JpegEncoder { Quality = 85 });
                            memoryStream.Position = 0;
                        }

                        // 将图片数据传给 iText
                        var imageData = iText.IO.Image.ImageDataFactory.Create(
                            memoryStream.ToArray()
                        );
                        iText.Layout.Element.Image img = new(imageData);

                        // 获取当前页面大小并让图片自适应铺满
                        img.SetAutoScaleWidth(true).SetAutoScaleHeight(true);
                        document.Add(img);

                        // 添加新页（除了最后一张图片，iText 会在添加图片时自动处理分页，但为了精准控制可以手动加）
                        document.Add(new AreaBreak());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[警告] 跳过损坏的图片: {ex.Message}");
                    }
                }
            }

            // 2. 添加中文书签（iText7 完美支持，无需配置任何字体！）
            int addedPages = pdfDocument.GetNumberOfPages() - startPageCount;
            if (addedPages > 0)
            {
                var title =
                    chapter
                        .Chaptername?.Split('\\', '/')
                        .LastOrDefault(x => !string.IsNullOrWhiteSpace(x))
                    ?? "未命名章节";

                // 获取该章节的第一页
                var firstPage = pdfDocument.GetPage(startPageCount + 1);

                // 创建跳转动作
                var action = PdfAction.CreateGoTo(PdfExplicitDestination.CreateFit(firstPage));

                // 添加大纲（书签）
                var outline = pdfDocument.GetOutlines(false).AddOutline(title);
                outline.AddAction(action);
            }
        }
    }
}
