//using iText.IO.Image;
//using iText.Kernel.Pdf;
//using iText.Kernel.Pdf.Action;
//using iText.Kernel.Pdf.Navigation;
//using iText.Layout;

using PdfSharp.Drawing;

using SixLabors.ImageSharp;
namespace EroMangaManager.Core.IOOperation;

/// <summary>
/// 对外导出功能
/// </summary>
public class Exporter
{
    /// <summary>
    /// 通过pdfsharp库导出pdf，MIT协议，有书签功能，但有些日文会乱码
    /// </summary>
    /// <param name="manga"></param>
    /// <param name="fileName"></param>
    public static void Export_PDFSharp(Manga manga, string fileName)
    {
        var pdf = new PdfSharp.Pdf.PdfDocument();
        foreach (var chapter in manga.Chapters)
        {
            foreach (var stream in chapter)
            {
                MemoryStream memoryStream = new();
                stream.CopyTo(memoryStream);
                memoryStream.Position = 0;

                var format = Image.DetectFormat(memoryStream);
                memoryStream.Position = 0;
                switch (format.Name)
                {
                    case "Webp":
                        {
                            var image = Image.Load(memoryStream);
                            memoryStream.SetLength(0);
                            image.SaveAsPng(memoryStream);
                            memoryStream.Position = 0;

                        }
                        break;

                    case "BMP":
                    case "JPEG":
                    case "PNG": break;

                    default:

                        continue;

                }

                var page = pdf.AddPage();
                XImage ximage;
                ximage = XImage.FromStream(memoryStream);  // 如果图片异常（是jpg，png，但解析失败），会报错：不支持的格式
                // TODO 添加一个异常保底图片 try-catch
                var gfx = XGraphics.FromPdfPage(page);
                page.Height = XUnit.FromPoint(ximage.Size.Height);
                page.Width = XUnit.FromPoint(ximage.Size.Width);
                gfx.DrawImage(ximage, 0, 0);

            }
            var index = pdf.PageCount - chapter.Count();
            var title = chapter.chaptername.Split('\\', '/').Last(x => !string.IsNullOrWhiteSpace(x));
            pdf.Outlines.Add(title, pdf.Pages[index]);
        }
        pdf.Save(fileName);

    }

    /// <summary>
    /// 调用itext的pdf库，这个库用的AGPL协议，污染性开源。
    /// 没做书签功能
    /// 已卸载nuget包
    /// </summary>
    /// <param name="manga"></param>
    /// <param name="destination"></param>
    //[Obsolete]
    //public static void Export_iText(Manga manga, string destination)
    //{
    //    var writestream = new FileStream(destination, FileMode.Create, FileAccess.Write);
    //    using var writer = new PdfWriter(writestream);
    //    using var pdfDocument = new iText.Kernel.Pdf.PdfDocument(writer);
    //    using var document = new Document(pdfDocument);
    //    document.SetMargins(0, 0, 0, 0);

    //    foreach (var chapter in manga.Chapters)
    //    {

    //        foreach (var stream in chapter)
    //        {
    //            // 这里需要复制出来，是因为itext pdf库不支持webp图片，需要先检测并对webp转格式，原stream不能seek，所以要复制出来
    //            MemoryStream memoryStream = new();
    //            stream.CopyTo(memoryStream);
    //            memoryStream.Position = 0;

    //            var format = Image.DetectFormat(memoryStream);
    //            memoryStream.Position = 0;

    //            if (format.Name == "Webp")
    //            {
    //                var image = Image.Load(memoryStream);
    //                memoryStream.SetLength(0);
    //                image.SaveAsPng(memoryStream);
    //                memoryStream.Position = 0;

    //            }

    //            var imageData = ImageDataFactory.Create(memoryStream.ToArray());

    //            var image1 = new iText.Layout.Element.Image(imageData);
    //            var width = pdfDocument.GetDefaultPageSize().GetWidth();
    //            image1.SetWidth(width)
    //            .SetAutoScaleHeight(true);

    //            document.Add(image1);
    //            stream.Dispose();
    //        }

    //        // 为pdf添加书签（outline），如果只有一个书签好像不能跳转，
    //        //    var index=chapter
    //        //var page = pdfDocument.GetPage(10);
    //        //var baseOutline = pdfDocument.GetOutlines(false);

    //        //PdfAction action = PdfAction.CreateGoTo(PdfExplicitDestination.CreateFit(page));
    //        //PdfOutline pdfOutline = baseOutline.AddOutline($"11111");
    //        //pdfOutline.SetOpen(false);
    //        ////pdfOutline.AddAction(action); // 会报错，不知道为什么
    //        //pdfOutline.AddDestination(PdfExplicitDestination.CreateFit(page));


    //    }

    //    pdfDocument.Close();

    //}
}