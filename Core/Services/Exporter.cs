using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Core.Services;

/// <summary>
/// 对外导出功能
/// </summary>
public partial class Exporter(MangaStreamProvider mangaStreamProviderm)
{
    /// <summary>
    /// 通过 PdfSharp 库导出 PDF
    /// </summary>
    public void Export_PDFSharp(Manga manga, string fileName)
    {
        // 在程序入口处，创建任何 XFont 对象之前调用
        // 能正常工作，但不需要了，指定字体
        //PdfSharp.Fonts.GlobalFontSettings.FontResolver = new CustomFontResolver();
        //var xFont = new XFont("HarmonyOS Sans SC" , 10 , XFontStyleEx.Regular, new XPdfFontOptions(PdfFontEncoding.Unicode)  /*【核心修复】强制使用 Unicode 编码*/);
        var pdf = new PdfDocument();
        foreach (var chapter in manga.Chapters)
        {
            // 记录当前章节开始前的页数，用于准确计算书签索引
            int startPageCount = pdf.PageCount;

            foreach (var stream in mangaStreamProviderm.GetStreams(chapter))
            {
                using (stream)
                {
                    try
                    {
                        // 确保源流位置在开头
                        if (stream.CanSeek)
                            stream.Position = 0;

                        using var memoryStream = new MemoryStream();
                        stream.CopyTo(memoryStream);
                        memoryStream.Position = 0;

                        var format = Image.DetectFormat(memoryStream);
                        memoryStream.Position = 0;

                        // 处理 WebP：转换为 JPEG 以减小体积
                        if (format.Name == "Webp")
                        {
                            using var image = Image.Load(memoryStream);
                            memoryStream.SetLength(0);
                            // 使用 JPEG 代替 PNG，大幅减小体积
                            image.Save(memoryStream, new JpegEncoder { Quality = 85 });
                            memoryStream.Position = 0;
                        }
                        else if (format.Name is not ("BMP" or "JPEG" or "PNG"))
                        {
                            continue;
                        }
                        var page = pdf.AddPage();
                        using var ximage = XImage.FromStream(memoryStream);

                        // 设置页面大小与图片一致
                        page.Height = XUnit.FromPoint(ximage.Size.Height);
                        page.Width = XUnit.FromPoint(ximage.Size.Width);

                        var gfx = XGraphics.FromPdfPage(page);
                        gfx.DrawImage(ximage, 0, 0);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[警告] 跳过损坏的图片: {ex.Message}");
                    }
                }
            }

            // 计算该章节实际添加的页数
            int addedPages = pdf.PageCount - startPageCount;
            if (addedPages > 0)
            {
                // 安全提取标题
                var title =
                    chapter
                        .Chaptername?.Split('\\', '/')
                        .LastOrDefault(x => !string.IsNullOrWhiteSpace(x))
                    ?? "未命名章节";
                // 书签指向各章节的第一页
                pdf.Outlines.Add(title, pdf.Pages[startPageCount]);
            }
        }

        pdf.Save(fileName);
    }
}

/// <summary>
/// 自定义字体解析器，用于在 PdfSharp 中使用自定义字体
/// </summary>
public class CustomFontResolver : IFontResolver
{
    // 1. 定义一个字典，将字体名称映射到字体文件路径
    private readonly Dictionary<string, string> _fontFiles = new()
    {
        // 键是你在代码中使用的字体名称，值是字体文件的绝对路径
        {
            "HarmonyOS Sans SC",
            @"E:\3D\字体\HarmonyOS_Sans（免费可商用）\HarmonyOS_Sans_SC_Regular.ttf"
        },
    };

    /// <summary>
    /// 实现 ResolveTypeface 方法
    /// 当 PdfSharp 需要解析字体时，会调用这个方法
    /// </summary>
    /// <param name="familyName"></param>
    /// <param name="isBold"></param>
    /// <param name="isItalic"></param>
    /// <returns></returns>
    public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        // 检查请求的字体名称是否在我们定义的字典中
        if (_fontFiles.ContainsKey(familyName))
        {
            // 返回一个 FontResolverInfo 对象，告诉 PdfSharp 使用哪个字体
            // 参数分别是：字体名称、是否模拟粗体、是否模拟斜体
            return new FontResolverInfo(familyName, false, false);
        }
        // 如果找不到，返回 null，PdfSharp 会尝试使用备用字体或抛出异常
        return null;
    }

    /// <summary>
    /// 实现 GetFont 方法
    /// 当 PdfSharp 需要字体的实际数据时，会调用这个方法
    /// </summary>
    /// <param name="faceName"></param>
    /// <returns></returns>
    public byte[]? GetFont(string faceName)
    {
        // 根据字体名称找到对应的文件路径
        if (_fontFiles.TryGetValue(faceName, out var value))
        {
            // 读取字体文件的所有字节并返回
            return File.ReadAllBytes(value);
        }
        return null;
    }
}
