namespace UnoLibrary.Services;

/// <summary>
/// 基于该平台的实例创建方法
/// </summary>
public class MangaFactory(
    ObservableCollectionVM ViewModel,
    CoverHelper coverHelper,
    CoverSetter coverSetter,
    MangaIO mangaFileIO
)
{
    /// <summary>ViewModel初始化</summary>
    public void GetAllFolders(IEnumerable<string> storageFolders)
    {
        ViewModel.MangasGroups.Clear();

        foreach (var folder in storageFolders)
        {
            //不存在则跳过
            if (Directory.Exists(folder))
            {
                MangasGroup mangasFolder = new(folder)
                {
                    Guid = System.Guid.NewGuid().ToString("N"),
                };
                ViewModel.MangasGroups.Add(mangasFolder);
            }
        }
    }

    public async Task StartInitial()
    {
        if (ViewModel.MangasGroups.Any(x => x.UpdateState == MangasGroupUpdateState.Busy))
        {
            return;
        }
        var group = ViewModel.MangasGroups.FirstOrDefault(x =>
            x.UpdateState == MangasGroupUpdateState.Ready
        );

        if (group is not null)
        {
            await InitialGroup2(group);

            await StartInitial();
        }
    }

    /// <summary>
    /// 创建所有manga实例，但是不设置cover，filesize属性（丢到backgroundcoversetter里面后台执行）
    /// </summary>
    /// <param name="mangasFolder"></param>
    /// <returns></returns>
    public async Task InitialGroup2(MangasGroup mangasFolder)
    {
        foreach (var group in ViewModel.MangasGroups)
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
                        .Where(x =>
                            SupportedType.MangaType.Contains(Path.GetExtension(x).ToLower())
                        )
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
                            Guid = Guid.NewGuid().ToString("N"),
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
    }
}
