using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using EroMangaManager.Models;
using EroMangaManager.ViewModels;

using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Font;

using Windows.Storage;

namespace EroMangaManager.Helpers
{
    internal class Exporter
    {
        internal static async Task ExportAsPDF (MangaBook mangaBook , StorageFile target)
        {
            List<byte[]> bytes = new List<byte[]>();


            using (ReaderVM reader = new ReaderVM(mangaBook))
            {
                await reader.Open();
              await  reader.SelectEntriesAsync(false,false);

                Stream stream1 = await target.OpenStreamForWriteAsync();

                using (PdfWriter writer = new PdfWriter(stream1))
                {
                    using (PdfDocument pdfDocument = new PdfDocument(writer))
                    {
                        using (Document document = new Document(pdfDocument))
                        {
                            foreach (var entry in reader.zipArchiveEntries)
                            {
                                var stream = entry.OpenEntryStream();
                                var b = new byte[stream.Length];
                                stream.Read(b , 0 , b.Length);

                                ImageData imageData = ImageDataFactory.Create(b);

                                Image image = new Image(imageData);
                                image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth());
                                image.SetAutoScaleHeight(true);

                                document.Add(image);

                            }
                        }
                    }
                }
            }
        }
    }
}