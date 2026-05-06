using EroMangaManager.Core.Setting;
using System.Threading.Tasks;

namespace EroMangaManager.Core.IOOperation;

/// <summary>
/// manga的io操作
/// </summary>
public class MangaFileIO
{
    /// <summary>
    /// 高级属性加载
    /// </summary>
    /// <param name="manga"></param>
    /// <returns></returns>
    public static async Task LoadMangaInfo(Manga manga)
    {
        manga.FileSize = await Task.Run(() => MangaFileIO.CountFileSize(manga));
        manga.Chapters.Clear();
        var chapters = await Task.Run(() => LoadChapter(manga));
        foreach (var chapter in chapters) { manga.Chapters.Add(chapter); }
        manga.ImageAmount = await Task.Run(() => MangaFileIO.CountImageAmount(manga));
        //manga.ChapterAmount = await Task.Run(() => MangaFileIO.CountChapterAmount(manga));
    }
    /// <summary>
    /// 统计漫画文件大小，压缩包则获取文件大小，文件夹则统计内部图片大小
    /// </summary>
    /// <param name="manga"></param>
    /// <returns></returns>
    public static long CountFileSize(Manga manga)
    {
        switch (manga.Type)
        {
            case "":
                {
                    return Directory
                        .EnumerateFiles(manga.FilePath, "*.*", new EnumerationOptions() { RecurseSubdirectories = true })
                        .Sum(x => new FileInfo(x).Length);
                }

            default:
                {
                    return new FileInfo(manga.FilePath).Length;

                    // filestream 也可以获取length;
                    //var rstr = new FileStream(manga.FilePath , FileMode.Open);
                }
        }
    }
    /// <summary>
    /// 漫画文件是否存在
    /// </summary>
    /// <param name="manga"></param>
    /// <returns></returns>
    public static bool Exists(Manga manga)
    {
        return Directory.Exists(manga.FilePath) || File.Exists(manga.FilePath);
    }
    /// <summary>
    /// 统计内部图片数量
    /// </summary>
    /// <param name="manga"></param>
    /// <returns></returns>
    public static int CountImageAmount(Manga manga)
    {
        switch (manga.Type)
        {
            case "":
                {
                    return Directory
      .EnumerateFiles(manga.FilePath, "*.*", new EnumerationOptions() { RecurseSubdirectories = true })
      .Count(x => SupportedType.ImageType.Contains(Path.GetExtension(x).ToLower()));
                }
            default:
                {
                    var archive = ArchiveFactory.Open(manga.FilePath);
                    var amount = archive.Entries
                         .Count(x => SupportedType.ImageType.Contains(Path.GetExtension(x.Key).ToLower()))
                    ;
                    archive.Dispose();
                    return amount;
                }
        }
    }
    /// <summary>
    /// 加载内部章节信息
    /// </summary>
    /// <param name="manga"></param>
    public static IEnumerable<Chapter> LoadChapter(Manga manga)
    {
        List<Chapter> chapters = [];
        switch (manga.Type)
        {
            case "":
                {
                    var files = Directory.EnumerateFiles(manga.FilePath)
                        .Where(x => SupportedType.ImageType.Contains(Path.GetExtension(x).ToLower()));
                    if (files.Any())
                    {
                        var chapter = new Chapter(manga)
                        {
                            chaptername = manga.MangaName,
                            chapterimagekey = files
                        };
                        chapters.Add(chapter);
                    }
                    var interfolders = Directory.EnumerateDirectories(manga.FilePath, "*", new EnumerationOptions { RecurseSubdirectories = true });
                    foreach (var folder in interfolders)
                    {
                        var folders = Directory.EnumerateFiles(folder)
                                                    .Where(x => SupportedType.ImageType.Contains(Path.GetExtension(x).ToLower()));

                        if (folders.Count() != 0)
                        {
                            var chapter = new Chapter(manga)
                            {
                                chaptername = folder,
                                chapterimagekey = folders

                            };
                            chapters.Add(chapter);
                        }
                    }
                }
                break;
            default:
                {
                    var archive = ArchiveFactory.Open(manga.FilePath);
                    var nonfolderentries = archive.Entries.Where(x => !x.IsDirectory).Where(x => SupportedType.ImageType.Contains(Path.GetExtension(x.Key).ToLower()))
.Select(x => x.Key).ToList();

                    var folderentries = archive.Entries.Where(x => x.IsDirectory).Select(x => x.Key).OrderByDescending(x => x.Length).ToList();

                    foreach (var folder in folderentries)
                    {

                        var items = nonfolderentries.Where(x => x.Contains(folder));
                        if (items.Any())
                        {
                            var chapter = new Chapter(manga) { chaptername = folder, chapterimagekey = items };
                            chapters.Add(chapter);
                            nonfolderentries = nonfolderentries.Except(items).ToList();

                        }

                    }
                    // TOTO 这里没有验证：剩下的是否就是直接包含在根目录下的
                    if (nonfolderentries.Count > 0)
                    {
                        var chapter = new Chapter(manga) { chaptername = manga.MangaName, chapterimagekey = nonfolderentries };
                        chapters.Add(chapter);

                    }


                }
                break;
        }
        return chapters;
    }
    /// <summary>
    /// 计算manga中的章节数，此方法只统计数量，并无具体章节信息
    /// </summary>
    /// <param name="manga"></param>
    /// <returns></returns>

    public static int CountChapterAmount(Manga manga)
    {
        var count = 0;
        switch (manga.Type)
        {
            case "":
                {
                    count = Directory.EnumerateDirectories(manga.FilePath, "*", new EnumerationOptions() { RecurseSubdirectories = true })
                         .Count(folder => SupportedType.ImageType.Any(imagetype => Directory.EnumerateFiles(folder).Any(file => Path.GetExtension(file).ToLower() == imagetype)));
                }
                break;

            default:
                {
                    var archive = ArchiveFactory.Open(manga.FilePath);
                    var folders = archive.Entries.Where(x => x.IsDirectory).ToList();
                    for (var index = folders.Count - 1; index >= 0; index--)
                    {
                        for (int index2 = index - 1; index2 >= 0; index2--)
                        {
                            var key1 = folders[index].Key;
                            var key2 = folders[index2].Key;
                            if (key1.Contains(key2))
                            {
                                folders.RemoveAt(index2);
                                break;
                            }
                            if (key2.Contains(key1))
                            {
                                folders.RemoveAt(index);
                                break;
                            }
                        }
                    }
                    var files = archive.Entries.Where(x => !x.IsDirectory);
                    archive.Dispose();
                    count = folders.Count(folder => files.Any(file => file.Key.Contains(folder.Key)));
                }
                break;
        }
        if (count == 0)
        {
            count = 1; // 如果图片直接存在压缩文件里或文件夹里面，count会为0
        }
        return count;
    }


    // 不需要单独写一个rename方法
    /// <summary>
    /// 移动、重命名漫画。
    /// </summary>
    /// <param name="book"></param>
    /// <param name="targetfolder">目标文件夹</param>
    /// <param name="newname">新名字</param>
    public static string MoveManga(Manga book, string targetfolder, string newname)
    {
        targetfolder ??= book.FolderPath; //这个路径并没有验证是否存在

        newname ??= book.FileDisplayName;
        var newpath = Path.Combine(targetfolder, newname + book.Type);

        // TODO 可能重名
        switch (book.Type)
        {
            case "":

                {
                    Directory.Move(book.FilePath, newpath);
                    return newpath;
                    //book.FilePath = newpath;
                    // 由于manganame使用MVVM绑定到了UI，所以跨线程，不能直接操作
                }

            default:

                {
                    File.Move(book.FilePath, newpath);
                    return newpath;
                    //book.FilePath = newpath;
                }
        }
    }
}