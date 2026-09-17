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

    public override async Task Delete(Manga manga)
    {
        switch (manga.Type)
        {
            case "":
                {
                    var folder = await StorageFolder.GetFolderFromPathAsync(manga.FilePath);

                    await folder.DeleteAsync(StorageDeleteOption.Default);
                }
                break;

            default:
                {
                    var file = await StorageFile.GetFileFromPathAsync(manga.FilePath);
                    await file.DeleteAsync();
                }
                break;
        }
    }

    /// <summary>
    /// 移动、重命名漫画。
    /// 文件夹：同盘且目标不存在时用 Directory.Move（快）；否则逐文件搬（跨卷安全）。
    /// </summary>
    /// <param name="manga"></param>
    /// <param name="targetfolder">目标文件夹</param>
    /// <param name="newname">新名字</param>
    /// <returns>新路径</returns>
    public override string MoveManga(Manga manga, string? targetfolder, string? newname)
    {
        targetfolder ??= manga.FolderPath;
        newname ??= manga.FileDisplayName;

        var newpath = Path.Combine(targetfolder, newname + manga.Type);

        if (
            string.Equals(
                Path.GetFullPath(manga.FilePath).TrimEnd(Path.DirectorySeparatorChar),
                Path.GetFullPath(newpath).TrimEnd(Path.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase
            )
        )
            return newpath;

        Directory.CreateDirectory(targetfolder);

        if (manga.Type != "")
        {
            if (File.Exists(newpath))
                throw new IOException($"目标已存在：{newpath}");
            File.Move(manga.FilePath, newpath);
            return newpath;
        }

        var sourceRoot = manga.FilePath;
        var targetRoot = newpath;

        if (!Directory.Exists(sourceRoot))
            throw new DirectoryNotFoundException($"源目录不存在：{sourceRoot}");

        // 源与目标路径重叠 → 拒绝
        if (IsSubPath(sourceRoot, targetRoot) || IsSubPath(targetRoot, sourceRoot))
            throw new IOException("源路径与目标路径重叠");

        bool sameVolume = IsSameVolume(sourceRoot, targetRoot);
        bool targetExists = Directory.Exists(targetRoot);

        if (sameVolume && !targetExists)
        {
            // 同盘 + 目标不存在 → 原子挪动
            Directory.Move(sourceRoot, targetRoot);
        }
        else
        {
            // 跨盘，或目标已存在需合并 → 预检（只查文件冲突）+ 逐文件搬
            EnsureNoConflict(sourceRoot, targetRoot);
            Directory.CreateDirectory(targetRoot);
            MoveDirectoryByFiles(sourceRoot, targetRoot);

            //Directory.Delete(sourceRoot, recursive: true);
            StorageFolder
                .GetFolderFromPathAsync(sourceRoot)
                .AsTask()
                .GetAwaiter()
                .GetResult()
                .DeleteAsync(StorageDeleteOption.Default)
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }

        return newpath;
    }

    /// <summary>
    /// 递归预检：源目录下每个子目录、每个文件，只要在目标下已存在，就抛异常。
    /// 保证任意冲突都不会导致「半移动」。
    /// </summary>
    /// <summary>
    /// 递归预检：源目录下每个文件，只要目标位置已存在同名文件（或同名目录）就抛异常。
    /// 目录已存在不算冲突（允许递归合并）。
    /// </summary>
    private static void EnsureNoConflict(string sourceRoot, string targetRoot)
    {
        // 文件：目标已有同名文件 / 同名目录 → 冲突
        foreach (var file in Directory.GetFiles(sourceRoot, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceRoot, file);
            var dest = Path.Combine(targetRoot, relative);

            if (File.Exists(dest))
                throw new IOException($"目标已存在同名文件：{dest}");
            if (Directory.Exists(dest))
                throw new IOException($"目标已存在同名目录，与文件冲突：{dest}");
        }

        // 目录：目标已有同名文件（占了目录的位置）→ 冲突；同名目录放行
        foreach (var dir in Directory.GetDirectories(sourceRoot, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(sourceRoot, dir);
            var dest = Path.Combine(targetRoot, relative);

            if (File.Exists(dest))
                throw new IOException($"目标已存在同名文件，与目录冲突：{dest}");
            // Directory.Exists(dest) → 允许合并，不报错
        }
    }

    /// <summary>递归把 source 内的子目录和文件搬到已存在的 dest 下。跨卷安全。</summary>
    private static void MoveDirectoryByFiles(string source, string dest)
    {
        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(Path.Combine(dest, Path.GetRelativePath(source, dir)));

        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            File.Move(file, Path.Combine(dest, Path.GetRelativePath(source, file)));
    }

    /// <summary>判断两个路径是否在同一卷（盘）上。</summary>
    private static bool IsSameVolume(string pathA, string pathB)
    {
        var rootA = Path.GetPathRoot(Path.GetFullPath(pathA));
        var rootB = Path.GetPathRoot(Path.GetFullPath(pathB));
        return string.Equals(rootA, rootB, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>判断 child 是否位于 parent 之下（含相等不算）。</summary>
    private static bool IsSubPath(string parent, string child)
    {
        var p =
            Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        var c =
            Path.GetFullPath(child).TrimEnd(Path.DirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        return c.StartsWith(p, StringComparison.OrdinalIgnoreCase);
    }
}
