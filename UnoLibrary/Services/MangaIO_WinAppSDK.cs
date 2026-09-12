using System;
using System.Collections.Generic;
using System.Text;

namespace UnoLibrary.Services;

public class MangaIO_WinAppSDK(CoverHelper coverHelper) : MangaIO
{
    public override async Task<string> GetCoverFile(Manga manga)
    {
        try
        {
            // 结合思路二的简洁性：用 switch 表达式直接路由
            string? coverpath = manga.Type switch
            {
                "" => LoadCoverFromInternalFolder(manga.FilePath),
                _ => await coverHelper.TryCreatCoverFileAsync(manga.FilePath, null),
            };

            // 结合思路一的防御性：如果是 null，直接返回默认封面，无需抛异常
            return coverpath ?? coverHelper.DefaultCoverUri;
        }
        catch (Exception)
        {
            // 发生任何异常（如文件损坏等），返回错误封面
            // TODO: 建议在这里加一行日志记录 ex.Message
            return coverHelper.ErrorCoverUri;
        }
    }
}
