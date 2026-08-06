using System.Threading;
using System.Threading.Tasks;

namespace Core.ViewModels;

/// <summary>
/// 所有需要持续观察的集合都放在这，ViewModel
/// </summary>
public class ObservableCollectionVM
{
    /// <summary>
    /// 出现无法解析的Manga时引发
    /// </summary>
    public event Action<string>? ErrorZipEvent;

    /// <summary>
    /// 删除本子源文件后引发的事件
    /// </summary>
    public event Action<Manga>? EventAfterDeleteMangaSource;

    /// <summary>
    /// 完成某项任务时引发
    /// </summary>
    public event Action<string>? WorkDoneEvent;

    /// <summary>
    /// 任务失败事件
    /// </summary>
    public event Action<string>? WorkFailedEvent;

    /// <summary>
    /// 访问被拒绝，通常因文件权限不足引发
    /// </summary>
    public event Action? AccessDeniedEvent; // TODO 没有验证这个事件及相关的try-catch能否正常工作

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
    /// 尝试移除一个本子文件，成功返回true，失败或未删除返回false
    /// </summary>
    /// <param name="mangaBook"></param>
    public bool RemoveManga(Manga mangaBook)
    {
        string folderpath = mangaBook.FolderPath;
        MangasGroup folder = MangasGroups.Single(x => x.FolderPath == folderpath);
        _ = SearchResultMangas.Remove(mangaBook);
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
    public Func<MangasGroup, Task> InitialGroup = null!;

    /// <summary>
    /// 开始初始化所有MangasGroup，会以自我递归的方式，初始化所有groups
    /// </summary>
    /// <returns></returns>
    public async Task StartInitial()
    {
        if (MangasGroups.Any(x => x.UpdateState == MangasGroupUpdateState.Busy))
        {
            return;
        }
        var group = MangasGroups.FirstOrDefault(x => x.UpdateState == MangasGroupUpdateState.Ready);

        if (group is not null)
        {
            await InitialGroup.Invoke(group);

            await StartInitial();
        }
    }

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

    #region 搜索ViewModel相关
    /// <summary>
    /// 对单个搜索结果执行事件
    /// </summary>
    public event Action<IEnumerable<Manga>> SearchResultNewAddMulti = null!;

    /// <summary>
    /// 对单个搜索结果执行事件
    /// </summary>
    public event Func<Manga, Task> SearchResultNewAddSingle = null!;

    /// <summary>
    /// 搜索需要内容
    /// </summary>
    public string SearchRequiredText { get; set; } = "";

    /// <summary>
    /// 搜索只能需要tag
    /// </summary>
    public ObservableCollection<string> SearchRequiredTags { get; } = [];

    //public List<string> SearchAllTags { get; set; } = [];
    /// <summary>
    /// 搜索可选tag
    /// </summary>
    public ObservableCollection<string> SearchAvailableTags { get; } = [];

    /// <summary>
    /// 搜索结果
    /// </summary>
    public ObservableCollection<Manga> SearchResultMangas { get; } = [];

    /// <summary>
    /// 2. 新增的异步流式方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tags"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<Manga> FilterMangasAsync(
        string? name,
        IEnumerable<string> tags,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default
    )
    {
        // 复用底层过滤逻辑
        foreach (var manga in FilterMangas(name, tags))
        {
            // 响应客户端断开连接
            ct.ThrowIfCancellationRequested();

            // 避免内存大集合过滤时阻塞线程
            await Task.Yield();

            yield return manga;
        }
    }

    /// <summary>
    /// 执行搜索内容，添加到可观察集合
    /// </summary>
    /// <returns></returns>
    public async Task Search()
    {
        // 直接调用纯逻辑方法，避免代码重复
        var result = FilterMangas(SearchRequiredText, SearchRequiredTags);

        // 更新 UI 状态（这部分对 ASP.NET Core 后端不可见）
        SearchResultMangas.Clear();
        foreach (var manga in result)
        {
            await SearchResultNewAddSingle.Invoke(manga!);
            await Task.Yield();
            SearchResultMangas.Add(manga);
        }
    }

    // 3. 抽取出的私有底层过滤逻辑（避免代码重复）
    private IEnumerable<Manga> FilterMangas(string? name, IEnumerable<string> tags)
    {
        // 基础校验。这一段一定要有，不然在ui界面无任何搜索条件时，会返回所有内容
        if (string.IsNullOrWhiteSpace(name) && (tags == null || !tags.Any()))
        {
            return [];
        }

        return MangasGroups
            .SelectMany(x => x.Mangas)
            .Where(x => x.Name.Contains(name?.Trim() ?? string.Empty))
            .Where(x => tags == null || !tags.Any() || tags.All(y => x.Tags.Contains(y)));
    }

    /// <summary>
    /// 【保留】原有供 UI 调用的搜索方法，行为完全不变。
    /// </summary>
    /// <summary>
    /// 筛选tag
    /// </summary>
    /// <param name="query"></param>
    public void FiltSearchTags(string query)
    {
        SearchAvailableTags.Clear();
        foreach (var tag in AllTags.Except(SearchRequiredTags))
        {
            if (tag.Contains(query))
                SearchAvailableTags.Add(tag);
        }
    }
    #endregion
}
