namespace Core.ViewModels;

/// <summary>
/// 所有需要持续观察的集合都放在这，ViewModel
/// </summary>
public class ObservableCollectionVM
{
    /// <summary>
    /// 删除本子源文件后引发的事件
    /// </summary>
    public event Action<Manga>? EventAfterDeleteMangaSource;

    /// <summary>
    /// 移动本子源文件后引发的事件
    /// </summary>
    public event Action<Manga>? EventAfterMoveMangaSource;

    /// <summary>
    /// 本子文件夹集合
    /// </summary>
    public ObservableCollection<MangasGroup> MangasGroups { get; } = [];

    /// <summary>
    /// 无法找到的文件夹
    /// </summary>
    public ObservableCollection<string> MissingFolders { set; get; } = [];

    /// <summary>存放zip文件的文件夹</summary>
    public List<string> StorageFolders => [.. MangasGroups.Select(n => n.FolderPath)];

    /// <summary>各漫画zip</summary>
    ///
    public IEnumerable<Manga> MangaList => MangasGroups.SelectMany(x => x.Mangas);

    /// <summary>
    /// 所有标签
    /// </summary>
    public IEnumerable<string> AllTags => MangaList.SelectMany(x => x.Tags).Distinct();

    /// <summary>
    /// MangasFolder是否正在更新，有任意一个是则返回true
    /// </summary>
    public bool IsContentInitializing =>
        MangasGroups.Any((x) => x.UpdateState == MangasGroupUpdateState.Busy);

    /// <summary>
    /// 确保已添加文件夹，并添加到集合。如果已存在这个folder，则返回true;否则返回false并创建新的
    /// </summary>
    /// <returns></returns>
    public bool EnsureAddFolder(string path, out MangasGroup mangasFolder)
    {
        if (StorageFolders.Contains(path))
        {
            mangasFolder = MangasGroups.Single(x => x.FolderPath == path);
            return true;
        }
        else
        {
            mangasFolder = new(path) { Guid = Guid.NewGuid().ToString("N") };
            MangasGroups.Add(mangasFolder);

            return false;
        }
    }

    /// <summary>
    /// 移除文件夹，并从集合中移除文件夹及下属漫画 （只移除，不删除）
    /// 1.从系统API中移除
    /// 2.从FolderList里移除
    /// 3.从MangaList里移除文件夹下属漫画
    /// </summary>
    public void RemoveFolder(MangasGroup group)
    {
        MangasGroups.Remove(group);
    }

    /// <summary>
    /// 删除manga后执行此事件
    /// </summary>
    /// <param name="manga"></param>
    public void InvokeEvent_AfterDeleteMnagaSource(Manga manga)
    {
        EventAfterDeleteMangaSource?.Invoke(manga);
    }

    /// <summary>
    /// 删除manga后执行此事件
    /// </summary>
    /// <param name="manga"></param>
    public void InvokeEvent_AfterMoveMnagaSource(Manga manga)
    {
        EventAfterMoveMangaSource?.Invoke(manga);
    }

    /// <summary>
    /// 尝试移除一个本子文件，成功返回true，失败或未删除返回false
    /// </summary>
    /// <param name="mangaBook"></param>
    public bool RemoveManga(Manga mangaBook)
    {
        string folderpath = mangaBook.FolderPath;
        MangasGroup folder = MangasGroups.Single(x => x.FolderPath == folderpath);
        return folder.RemoveManga(mangaBook);
    }

    /// <summary>
    /// 开始初始化所有MangasGroup，会以自我递归的方式，初始化所有groups
    /// </summary>
    /// <returns></returns>
    /// <summary>
    /// 把一个本子放到他应该在的集合里面，这个一般用在移动本子后
    /// </summary>
    /// <param name="manga"></param>
    public void PlaceInCorrectGroup(Manga manga)
    {
        // 之前时在一个foreach里又套一个foreach对同一个集合进行嵌套遍历，导致出问题
        var oldgroup = MangasGroups.Single(x => x.Mangas.Contains(manga));
        if (oldgroup.RemoveManga(manga))
        {
            var newgroup = MangasGroups.Single(x => x.FolderPath == manga.FolderPath);
            if (!newgroup.Mangas.Contains(manga))
            {
                newgroup.AddManga(manga);
            }
        }
    }
}
