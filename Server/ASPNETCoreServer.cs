
using EroMangaManager.Core.DTOs;
using EroMangaManager.Core.Models;
using EroMangaManager.Core.ViewModels;
using PdfSharp.Pdf.Filters;
using SharpCompress.Archives;

namespace EroMangaManager.Server;

public class ASPNETCoreServer
{
    Task self;
    readonly EroMangaManager.Core.ViewModels.ObservableCollectionVM viewmodel;
    public event Action<Manga> EventDeleteMang;
    public ASPNETCoreServer(ObservableCollectionVM collectionVM)
    {
        viewmodel = collectionVM;

    }


    public async Task StartServer()
    {
        if (self?.Status == TaskStatus.Running) return;
        var builder = WebApplication.CreateBuilder();

        // 添加这行代码，允许同步 IO，downloads中使用的是同步方法
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AllowSynchronousIO = true;
        });
        // 添加这行代码，允许同步 IO
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.AllowSynchronousIO = true;
        });
        var app = builder.Build();
        app.Urls.Add("http://*:12965");
        app.MapGet("/api/health", () => Results.Ok());
        app.MapGet("/tags", () => Results.Ok(viewmodel.AllTags));

        app.MapGet("/folders/basicinfo", () => Results.Ok(viewmodel.MangaFolders.Select(x => new MangasGroupDTO( x) )));

        app.MapGet("/folders/{guid}", (string guid) =>
        {
            var folder = viewmodel.MangaFolders.FirstOrDefault(x => x.Guid == guid);
            if (folder != null)
            {
                var folderDTO =new MangasGroupDTO(folder);
                return Results.Ok(folderDTO);
            }
            else
            {
                return Results.NotFound();
            }
        });
        app.MapGet("/folders/{guid}/count", (string guid) =>
        {
            var group = viewmodel.MangaFolders.FirstOrDefault(x => x.Guid == guid);
            if (group != null)
            {
                return Results.Ok(group.Mangas.Count);
            }
            else
            {
                return Results.NotFound();
            }
        });
        app.MapGet("/folders/{guid}/{index}/{amount}", (string guid, int index, int amount) =>
        {
            var group = viewmodel.MangaFolders.FirstOrDefault(x => x.Guid == guid);
            if (group != null)
            {
                
                var dtos =(group.Mangas.Skip(index).Take(amount).Select(x => new MangaDTO(x)));
                return Results.Ok(dtos);
            }
            else
            {
                return Results.NotFound();
            }
        });
        app.MapGet("/mangas", () => Results.Ok(viewmodel.MangaList.Select(x =>new MangaDTO(x))));

        app.MapGet("/mangas/{guid}", (string guid) =>
        {
            var manga = viewmodel.MangaList.FirstOrDefault(x => x.Guid == guid);

            if (manga != null)
            {
                var mangaDTO =new MangaDTO(manga);
                return Results.Ok(mangaDTO);
            }
            else
            {
                return Results.NotFound();
            }
        });


        app.MapGet("downloads/{guid}", async (string guid, HttpContext context) =>
        {
            var manga = viewmodel.MangaList.FirstOrDefault(x => x.Guid == guid);
            switch (manga.Type)
            {
                case "":
                    var archive = SharpCompress.Archives.Zip.ZipArchive.Create();
                    archive.AddAllFromDirectory(manga.FilePath);

                    //context.Response.ContentLength = manga.FileSize;
                    context.Response.Headers["X-Estimated-Size"] = manga.FileSize.ToString();
                    //context.Response.Headers.Append("X-Estimated-Size", manga.FileSize.ToString());
                    //context.Response.Headers.Append("Access-Control-Expose-Headers", "X-Estimated-Size");
                    archive.SaveTo(context.Response.Body, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None));

                    //context.Response.Body.Position = 0;
                    // 结束响应
                    await context.Response.CompleteAsync();
                    return Results.Empty;
                default:
                    return Results.File(manga.FilePath);
            }
        });
        #region 另一种传输方式，有问题，会中断连接
        //有问题，会中断连接
        //       app.MapGet("/downloads2/{guid}", async (string guid, HttpContext context) =>
        //       {
        //           var manga = viewmodel.MangaList.FirstOrDefault(x => x.Guid == guid);
        //           if(manga != null && manga.Type != "")
        //           {
        //               return Results.File(manga.FilePath, contentType: "application/octet-stream", fileDownloadName: $"{manga.Guid}.zip");
        //           }

        //           //MemoryStream stream = new();
        //           //Stream outputStream = context.Response.Body;

        //                   // 1. 获取文件夹路径
        //                   var directoryPath = manga.FilePath;
        //                   if (!Directory.Exists(directoryPath)) return Results.NotFound();

        //           // 2. 获取所有图片文件 (支持常见格式，按需添加)
        //           var files = Directory.GetFiles(directoryPath, "*", new EnumerationOptions() { RecurseSubdirectories = true })
        //                                //.Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
        //                                //            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
        //                                //            f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
        //                                .OrderBy(f => f) // 建议排序，保证压缩包内顺序一致
        //   .ToList();
        //                  // 3. 设置响应头
        //                   context.Response.ContentType = "application/zip";
        //           context.Response.ContentLength = manga.FileSize;

        //           context.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{manga.Guid}.zip\"");
        //                   // 4. 创建 ZipArchive (直接写入 Response.Body)
        //                   // 注意：这里不需要 using，因为当方法结束时，ASP.NET Core 会自动关闭 Response.Body
        //                   using (var archive = new ZipArchive(context.Response.Body, ZipArchiveMode.Create, leaveOpen: true))
        //                   {
        //                       foreach (var filePath in files)
        //                       {
        //                           // 5. 为每个文件创建一个 Zip 条目
        //                           // 这里使用 Path.GetFileName 确保压缩包里没有绝对路径
        //                           var entry = archive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.NoCompression);

        //                           // 6. 打开源文件的读取流 (File.OpenRead 是异步友好的)
        //                           using (var fileStream = File.OpenRead(filePath))
        //                           {
        //                               // 7. 打开 Zip 条目的写入流
        //                               using (var entryStream = entry.Open())
        //                               {
        //                                   // 8. 核心：将文件流异步复制到 Zip 流中
        //                                   // 这一步是“流式”的关键：读一点 -> 压一点 -> 发一点
        //                                   await fileStream.CopyToAsync(entryStream);
        //                               }
        //                           }
        //                       }
        //                   }
        //           context.Response.Body.Position = 0;
        //                   // 结束响应
        //                   //await context.Response.CompleteAsync();
        //           return Results.Empty;
        //       }
        //);
        #endregion
        //app.MapGet("/covers/all", () => Results.Ok(viewmodel.MangaList.Select(x => x.CoverPath)));
        app.MapGet("/covers/{guid}", (string guid) =>
        {
            var file = viewmodel.MangaList.SingleOrDefault(x => x.Guid == guid)?.CoverPath;

            return file is null ? Results.NotFound() : Results.File(file);
        });

        app.MapDelete("/mangas/{guid}", async (string guid) =>
        {
            var manga = viewmodel.MangaList.SingleOrDefault(x => x.Guid == guid);
            if (manga != null&& guid != null)
            {
                
                this.EventDeleteMang?.Invoke(manga);
                return Results.Ok();
            }
            else
            {
                return Results.NotFound();
            }
        });
        self = app.RunAsync();

    }
}
