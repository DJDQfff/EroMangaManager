using System.Text;
using Core.Services;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Core.Models;

/// <summary>
/// 根据 Chapter 的数据，动态生成图片流
/// 用于配合 <see cref="Chapter.Chapterimagekey"/> 使用，以前是在一起的，为序列化拆开。
/// </summary>
public class MangaStreamProvider
{
    /// <summary>
    /// 根据 Chapter 的数据，动态生成图片流
    /// </summary>
    public IEnumerable<Stream> GetStreams(Chapter chapter)
    {
        if (chapter?.Manga == null || chapter.Chapterimagekey == null)
            yield break;

        // 如果是压缩包，在循环外只打开一次，避免内存泄漏和重复IO
        IArchive archive = null!;
        if (!string.IsNullOrEmpty(chapter.Manga.Type))
        {
            var result = ZipEncodingDetector.IsAllEntriesUseUtf8(chapter.Manga.FilePath);
            ReaderOptions options = new();
            if (!result)
            {
                options.ArchiveEncoding = new ArchiveEncoding
                {
                    Default = Encoding.GetEncoding("GBK"),
                };
            }
            ;

            archive = ArchiveFactory.OpenArchive(chapter.Manga.FilePath, options);
        }

        try
        {
            foreach (var str in chapter.Chapterimagekey)
            {
                switch (chapter.Manga.Type)
                {
                    case "": // 文件夹
                        // FileStream 支持 Seek，PdfSharp 可以直接处理
                        yield return new FileStream(str, FileMode.Open, FileAccess.Read);
                        break;

                    default: // 压缩包
                        {
                            var entry = archive.Entries.SingleOrDefault(x => x.Key == str);

                            if (entry is null)
                            {
                                yield break;
                            }
                            else
                            {
                                yield return entry.OpenEntryStream();
                            }
                        }
                        break;
                }
            }
        }
        finally
        {
            // 注意：因为使用了 yield return，这里的 finally 会在枚举器被 Dispose 时执行
            // 确保压缩包在使用完毕后被正确关闭
            archive?.Dispose();
        }
    }
}
