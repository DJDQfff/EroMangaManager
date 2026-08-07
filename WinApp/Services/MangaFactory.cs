using System.Diagnostics;
using Core.Services;

namespace UnoLibrary.Services;

/// <summary>
/// 基于该平台的实例创建方法
/// </summary>
public class MangaFactory(CoverHelper coverHelper, CoverSetter coverSetter, MangaFileIO mangaFileIO)
{
    /// <summary>ViewModel初始化</summary>
    public void GetAllFolders(ObservableCollectionVM ViewModel, IEnumerable<string> storageFolders)
    {
        ViewModel.MangasGroups.Clear();

        foreach (var folder in storageFolders)
        {
            //不存在则跳过
            if (Directory.Exists(folder))
            {
                var mangasFolder = new MangasGroup(folder)
                {
                    Guid = System.Guid.NewGuid().ToString("N"),
                };
                ViewModel.MangasGroups.Add(mangasFolder);
            }
        }
    }

    /// <summary>
    /// 创建所有manga实例，但是不设置cover，filesize属性（丢到backgroundcoversetter里面后台执行）
    /// </summary>
    /// <param name="mangasFolder"></param>
    /// <returns></returns>
    public async Task InitialGroup2(MangasGroup mangasFolder)
    {
        if (Directory.Exists(mangasFolder.FolderPath))
        {
            mangasFolder.UpdateState = MangasGroupUpdateState.Busy;
            List<Manga> list = [];
            //var a = DatabaseController.database.FilteredImages.ToArray();
            //所有子文件作为mangabook
            var filteredfiles = await Task.Run(() =>
                Directory
                    .EnumerateFiles(mangasFolder.FolderPath)
                    .Where(x => SupportedType.MangaType.Contains(Path.GetExtension(x).ToLower()))
                    .Select(xfile => new Manga(xfile)
                    {
                        CoverUri = coverHelper.DefaultCoverUri,
                        Guid = System.Guid.NewGuid().ToString("N"),
                    })
            );
            list.AddRange(filteredfiles);
            //foreach (var manga in filteredfiles)
            //{
            //    mangasFolder.Mangas.Add(manga);
            //}
            //所有子文件夹作为mangabook
            Stopwatch stopwatch = new();
            stopwatch.Start();
            var folders = await Task.Run(() =>
                Directory
                    .EnumerateDirectories(mangasFolder.FolderPath)
                    .Select(x => new Manga(x)
                    {
                        CoverUri = coverHelper.DefaultCoverUri,
                        Guid = System.Guid.NewGuid().ToString("N"),
                    })
            );
            stopwatch.Stop();
            Debug.WriteLine(mangasFolder.FolderPath);
            Debug.WriteLine(stopwatch.ElapsedMilliseconds);
            list.AddRange(folders);
            //foreach (var manga in folders)
            //{
            //    mangasFolder.Mangas.Add(manga);
            //    //App.Current.BackgroundCoverSetter.mangas.Add(manga);
            //}
            mangasFolder.AddManga(list);
            //mangasFolder.Filter(null , 0 , 0);
            mangasFolder.Display(0, 20);
            await coverSetter.MultiLoadWork(mangasFolder.DisplayMangas, true, true);
            mangasFolder.UpdateState = MangasGroupUpdateState.Over;

            //await App.Current.CoverSetter.AppendLoadWorks(mangasFolder.Mangas,false,true);
        }
    }

    public async Task<string> GetCoverFile(Manga manga)
    {
        try
        {
            // 结合思路二的简洁性：用 switch 表达式直接路由
            string? coverpath = manga.Type switch
            {
                "" => mangaFileIO.LoadCoverFromInternalFolder(manga.FilePath),
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
