using System.Threading.Tasks;

namespace EroMangaManager.Core.ViewModels;

/// <summary>
/// 所有需要持续观察的集合都放在这，ViewModel
/// </summary>
public class ObservableCollectionVM
{
    /// <summary>
    /// 出现无法解析的Manga时引发
    /// </summary>
    public event Action<string> ErrorZipEvent;

    /// <summary>
    /// 删除本子源文件后引发的事件
    /// </summary>
    public event Action<Manga> EventAfterDeleteMangaSource;

    /// <summary>
    /// 完成某项任务时引发
    /// </summary>
    public event Action<string> WorkDoneEvent;

    /// <summary>
    /// 任务失败事件
    /// </summary>
    public event Action<string> WorkFailedEvent;

    /// <summary>
    /// 访问被拒绝，通常因文件权限不足引发
    /// </summary>
    public event Action AccessDeniedEvent; // TODO 没有验证这个事件及相关的try-catch能否正常工作

    
    /// <summary>
    /// 本子文件夹集合
    /// </summary>
    public ObservableCollection<MangasGroup> MangaFolders { get; } = [];

    /// <summary>
    /// 无法找到的文件夹
    /// </summary>
    public ObservableCollection<string> MissingFolders { set; get; } = [];

    /// <summary>存放zip文件的文件夹</summary>
    public List<string> StorageFolders => MangaFolders.Select(n => n.FolderPath).ToList();

    /// <summary>各漫画zip</summary>
    ///
    public List<Manga> MangaList
    {
        get
        {
            List<Manga> list = [];
            foreach (MangasGroup folder in MangaFolders)
            {
                list.AddRange(folder.Mangas);
            }
            return list;
        }
    }

    /// <summary>
    /// 所有标签
    /// </summary>
    public IEnumerable<string> AllTags
    {
        get
        {
            List<string> alltags = [];
            foreach (var manga in MangaList)
            {
                var tags = manga.Tags;
                alltags.AddRange(tags);
            }
            return alltags.Distinct();

            ;
        }
    }

    /// <summary>
    /// MangasFolder是否正在更新，有任意一个是则返回true
    /// </summary>
    public bool IsContentInitializing => MangaFolders.Any((x) => x.UpdateState == MangasGroupUpdateState.Busy);

    /// <summary>
    /// 确保已添加文件夹，并添加到集合。如果已存在这个folder，则返回true;否则返回false并创建新的
    /// </summary>
    /// <returns></returns>
    public bool EnsureAddFolder(string path, out MangasGroup mangasFolder)
    {
        if (StorageFolders.Contains(path))
        {
            mangasFolder = MangaFolders.Single(x => x.FolderPath == path);
            return true;
        }
        else
        {
            mangasFolder = new MangasGroup(path);
            mangasFolder. Guid = System.Guid.NewGuid().ToString("N");
            MangaFolders.Add(mangasFolder);

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
        MangaFolders.Remove(group);
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
    /// 尝试移除一个本子文件，成功返回true，失败或未删除返回false
    /// </summary>
    /// <param name="mangaBook"></param>
    public bool RemoveManga(Manga mangaBook)
    {
        string folderpath = mangaBook.FolderPath;
        MangasGroup folder = MangaFolders.Single(x => x.FolderPath == folderpath);

        return folder.RemoveManga(mangaBook);
    }

    /// <summary>
    /// 事情完成时发生
    /// </summary>
    /// <param name="message"></param>
    public void WorkDone(string message) => WorkDoneEvent?.Invoke(message);

    /// <summary>
    /// 任务失败
    /// </summary>
    /// <param name="message"></param>
    public void WorkFailed(string message) => WorkFailedEvent?.Invoke(message);

    /// <summary>
    /// 触发访问被拒绝异常
    /// </summary>
    public void AccessDenied() => AccessDeniedEvent?.Invoke();
    /// <summary>
    /// 发现错误漫画时引发
    /// </summary>
    /// <param name="manganame"></param>
    public void ErrorMangaEvent(string manganame)
    {
        ErrorZipEvent?.Invoke(manganame);
    }

    /// <summary>
    /// 后台更新MangasGroup的Func
    /// </summary>
    public Func<MangasGroup, Task> InitialGroup;

    /// <summary>
    /// 开始初始化所有MangasGroup，会以自我递归的方式，初始化所有groups
    /// </summary>
    /// <returns></returns>
    public async Task StartInitial()
    {
        if (MangaFolders.Any(x => x.UpdateState == MangasGroupUpdateState.Busy))
        {
            return;
        }
        var group = MangaFolders.FirstOrDefault(x => x.UpdateState == MangasGroupUpdateState.Ready);

        if (group is not null)
        {
            await InitialGroup.Invoke(group);

            await StartInitial();
        }
    }

    /// <summary>
    /// 把一个本子放到他应该在的集合里面，这个一般用在移动本子后
    /// </summary>
    /// <param name="book"></param>
    public void PlaceInCorrectGroup(Manga book)
    {
        foreach (var group in MangaFolders)
        {
            if (group.RemoveManga(book))
            {
                var g = MangaFolders.SingleOrDefault(x => x.FolderPath == book.FolderPath);
                if (g is not null)
                {
                    if (!g.Mangas.Contains(book))
                    { g.Mangas.Add(book); }
                }
            }
        }
    }
}