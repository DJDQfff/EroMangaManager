﻿using System;
using System.IO;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.ViewModels;

using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

using Windows.Storage;

namespace EroMangaManager.UWP.Helpers
{
    internal class Exporter
    {
        internal static async Task ExportAsPDF(MangaBook mangaBook, StorageFile target)
        {
            using (ReaderVM reader = new ReaderVM(mangaBook, target))
            {
                await reader.Initial();
                reader.SelectEntries(null);

                Stream stream1 = await target.OpenStreamForWriteAsync();

                using (PdfWriter writer = new PdfWriter(stream1))
                {
                    using (PdfDocument pdfDocument = new PdfDocument(writer))
                    {
                        using (Document document = new Document(pdfDocument))
                        {
                            foreach (var entry in reader.FilteredArchiveImageEntries)
                            {
                                using (var stream = entry.OpenEntryStream())
                                {
                                    using (MemoryStream memoryStream = new MemoryStream())
                                    {
                                        stream.CopyTo(memoryStream);
                                        var b = new byte[memoryStream.Length];
                                        memoryStream.Position = 0;
                                        memoryStream.Read(b, 0, b.Length);

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
    }
}