using System;
using System.IO;
using System.Text;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Core.Services;

/// <summary>
/// 手动检测 ZIP 压缩包内的文件名是否全部为合法的 UTF-8 编码
/// </summary>
public static class ZipEncodingDetector
{
    /// <summary>
    /// 检测 ZIP 压缩包内的文件名是否全部为合法的 UTF-8 编码
    /// </summary>
    public static bool IsAllEntriesUseUtf8(string zipPath)
    {
        if (!File.Exists(zipPath))
            return false;

        try
        {
            // 确保 .NET Core 支持 GBK 等代码页编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 1. 定义一个严格模式的 UTF-8 解码器（遇到非法字节直接抛异常）
            var strictUtf8 = new UTF8Encoding(false, true);

            // 2. 配置 SharpCompress 读取选项
            var options = new ReaderOptions
            {
                ArchiveEncoding = new ArchiveEncoding
                {
                    CustomDecoder = (bytes, index, count, encoding) =>
                    {
                        var fileNameBytes = new byte[count];
                        Array.Copy(bytes, index, fileNameBytes, 0, count);

                        if (fileNameBytes.Length == 0)
                            return string.Empty;

                        // 尝试用严格 UTF-8 解码，如果失败会自动抛出 DecoderFallbackException
                        return strictUtf8.GetString(fileNameBytes);
                    },
                },
            };

            // 3. 打开压缩包并遍历条目
            // 注意：这里使用 ZipArchive.Open 而不是 OpenArchive
            using var archive = ZipArchive.OpenArchive(zipPath, options);

            foreach (var entry in archive.Entries)
            {
                try
                {
                    // 触发 CustomDecoder 进行解码
                    var name = entry.Key;
                }
                catch (DecoderFallbackException)
                {
                    // 只要有一个文件名的 UTF-8 解码失败，就说明不是全 UTF-8
                    return false;
                }
            }

            // 所有文件名都成功通过了严格的 UTF-8 解码
            return true;
        }
        catch (Exception)
        {
            // 文件损坏、不是 ZIP 格式等其他异常，按 false 处理
            return false;
        }
    }
}
