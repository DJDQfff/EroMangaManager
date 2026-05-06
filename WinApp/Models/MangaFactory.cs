
namespace EroMangaManager.WinUI3.Models;

/// <summary>
/// 基于该平台的实例创建方法
/// </summary>
internal static class MangaFactory
{

    /// <summary>ViewModel初始化</summary>
    public static void GetAllFolders(
        this ObservableCollectionVM ViewModel,
        IEnumerable<string> storageFolders
    )
    {
        ViewModel.MangaFolders.Clear();

        foreach (var folder in storageFolders)
        {
            //不存在则跳过
            if (Directory.Exists(folder))
            {
                var mangasFolder = new MangasGroup(folder);
                mangasFolder. Guid = System.Guid.NewGuid().ToString("N");
                ViewModel.MangaFolders.Add(mangasFolder);
            }
        }
    }

    /// <summary>
    /// 创建所有manga实例，但是不设置cover，filesize属性（丢到backgroundcoversetter里面后台执行）
    /// </summary>
    /// <param name="mangasFolder"></param>
    /// <param name="StorageFolder"></param>
    /// <returns></returns>
    public static async Task InitialGroup2(this MangasGroup mangasFolder)
    {
        if (Directory.Exists(mangasFolder.FolderPath))
        {
            mangasFolder.UpdateState = MangasGroupUpdateState.Busy;
            List<Manga> list = [];
            //var a = DatabaseController.database.FilteredImages.ToArray();
            //所有子文件作为mangabook
            var filteredfiles = await Task.Run(() =>
                 Directory.EnumerateFiles(mangasFolder.FolderPath)
                  .Where(x => SupportedType.MangaType.Contains(Path.GetExtension(x).ToLower()))
                  .Select(xfile =>
                       new Manga(xfile)
                       {
                           CoverPath = CoverHelper.DefaultCoverPath,
                           Guid = System.Guid.NewGuid().ToString("N")

                       }
                  )
            );
            list.AddRange(filteredfiles);
            //foreach (var manga in filteredfiles)
            //{
            //    mangasFolder.Mangas.Add(manga);
            //}
            //所有子文件夹作为mangabook
            var folders = await Task.Run(() => Directory.EnumerateDirectories(mangasFolder.FolderPath)

                .Select(x =>
                     new Manga(x)
                     {

                         CoverPath = CoverHelper.DefaultCoverPath,
                         Guid = System.Guid.NewGuid().ToString("N")
                     }
                ));

            list.AddRange(folders);
            //foreach (var manga in folders)
            //{
            //    mangasFolder.Mangas.Add(manga);
            //    //App.Current.BackgroundCoverSetter.mangas.Add(manga);
            //}
            mangasFolder.Mangas = new(list);
            mangasFolder.UpdateState = MangasGroupUpdateState.Over;
        }
    }

    /// <summary>
    /// 依次创建manga实例、设置封面、filesize，
    /// </summary>
    /// <param name="mangasFolder"></param>
    /// <param name="StorageFolder"></param>
    /// <returns></returns>
    //public static async Task InitialGroup (this MangasGroup mangasFolder)
    //{
    //    if (Directory.Exists(mangasFolder.FolderPath))
    //    {
    //        mangasFolder.UpdateState = UpdateState.Busy;

    //        //var a = DatabaseController.database.FilteredImages.ToArray();
    //        List<Task> tasks = [];
    //        //所有子文件作为mangabook
    //        var files = Directory.EnumerateFiles(mangasFolder.FolderPath);
    //        var filteredfiles = files.Where(
    //            x => SupportedType.MangaType.Contains(Path.GetExtension(x).ToLower())
    //        );
    //        foreach (var xfile in filteredfiles)
    //        {
    //            var manga = new Manga(xfile)
    //            {
    //                Type = Path.GetExtension(xfile).ToLower()
    //            };

    //            await Task.Run(() =>
    //        {
    //            var fileinfo = new FileInfo(manga.FilePath);
    //            manga.FileSize = fileinfo.Length;
    //            fileinfo = null;
    //            manga.CoverPath = CoverHelper.DefaultCoverPath;
    //            //manga.CoverPath =
    //            //    await CoverHelper.TryCreatCoverFileAsync(manga.FilePath , null)
    //            //    ?? CoverHelper.DefaultCoverPath;

    //        });
    //            mangasFolder.Mangas.Add(manga);
    //            App.Current.BackgroundCoverSetter.mangas.Add(manga);

    //        }
    //        //所有子文件夹作为mangabook
    //        var folders = Directory.EnumerateDirectories(mangasFolder.FolderPath);
    //        foreach (var mangafo in folders)
    //        {
    //            var manga = new Manga(mangafo)
    //            {
    //                Type = string.Empty
    //            };
    //            await Task.Run(() =>
    //            {
    //                manga.FileSize = Directory
    //                    .GetFiles(manga.FilePath , "*.*" , new EnumerationOptions() { RecurseSubdirectories = true })
    //                    .Sum(x => new FileInfo(x).Length);
    //                manga.CoverPath = CoverHelper.DefaultCoverPath;

    //                //manga.CoverPath =
    //                //    CoverHelper.LoadCoverFromInternalFolder(manga.FilePath)
    //                //    ?? CoverHelper.DefaultCoverPath;

    //            });

    //            mangasFolder.Mangas.Add(manga);
    //            App.Current.BackgroundCoverSetter.mangas.Add(manga);

    //        }
    //        mangasFolder.UpdateState = UpdateState.Over;

    //    }
    //}






    public static async Task<string> GetCoverFile(Manga manga)
    {
        switch (manga.Type)
        {
            case "":
                {
                    return
                       CoverHelper.LoadCoverFromInternalFolder(manga.FilePath)
                        ?? CoverHelper.DefaultCoverPath;
                }

            default:
                {
                    return
                        await CoverHelper.TryCreatCoverFileAsync(manga.FilePath, null)
                        ?? CoverHelper.DefaultCoverPath;
                }
        }
    }


    [Obsolete]
    public static void InitialEachFoldersInOrder(this ObservableCollectionVM ViewModel)
    {
        // TODO 如果在初始化的时候，移除了这个文件夹，会出错，比如一些大型文件夹
        //foreach (var folder in ViewModel.MangaFolders.ToArray())
        //{
        //    var token = new CancellationTokenSource();
        //    App.Current.Tokens.TryAdd(folder, token);

        //    await folder.InitialGroup();
        //}
    }
}