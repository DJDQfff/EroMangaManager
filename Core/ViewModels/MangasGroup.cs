using System.Diagnostics.CodeAnalysis;

namespace Core.ViewModels;

/// <summary>
/// 本子组
/// </summary>
public partial class MangasGroup : ObservableObject
{
    /// <summary>
    ///
    /// </summary>
    public string Guid { get; set; } = string.Empty;

    /// <summary>
    /// 文件夹路径，一开始是作为文件夹设计的，后来不作为文件夹，仅作为本子统一集合
    /// </summary>
    public string FolderPath { get; set; } = string.Empty;

    ///// <summary>
    ///// 更新状态，表示是否再后台更新
    ///// </summary>
    [ObservableProperty]
    public partial MangasGroupUpdateState UpdateState { get; set; } = MangasGroupUpdateState.Ready;

    /// <summary>
    /// 本子集合
    /// </summary>
    public List<Manga> Mangas { get; set; } = [];

    /// <summary>
    ///
    /// </summary>
    public List<Manga> FilteredMangas { private set; get; } = [];

    /// <summary>
    /// 展示在UI中的Manga
    /// </summary>
    public ObservableCollection<Manga> DisplayMangas { get; set; } = [];

    /// <summary>
    /// 所有标签（已简单去重）
    /// </summary>
    public IEnumerable<string> AllTags => Mangas.SelectMany(x => x.Tags).Distinct();

    [ObservableProperty]
    public partial int Count { get; set; }

    /// <summary>
    /// 不当文件夹用，所以不指定文件夹路径
    /// </summary>
    public MangasGroup() { }

    /// <summary>
    /// 当文件夹用，需要指定文件夹路径
    /// </summary>
    /// <param name="storageFolderPath"></param>
    [SetsRequiredMembers]
    public MangasGroup(string storageFolderPath)
    {
        FolderPath = storageFolderPath;
    }

    /// <summary>
    /// 添加源内容
    /// </summary>
    /// <param name="mangas"></param>
    /// <returns></returns>
    public int AddManga(params IList<Manga> mangas)
    {
        Mangas.AddRange(mangas);
        Count = Mangas.Count;
        return Count;
    }

    /// <summary>
    /// 移除一个本子
    /// </summary>
    /// <param name="mangaBook"></param>
    public bool RemoveManga(Manga mangaBook)
    {
        _ = DisplayMangas.Remove(mangaBook);
        Count = Mangas.Count;

        return Mangas.Remove(mangaBook);
    }

    /// <summary>
    /// 对内部漫画进行排序
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="func"></param>
    public void SortMangas<TKey>(Func<Manga, TKey> func)
    {
        // TODO 这里没调用func
        Mangas.Sort((x, y) => x.FileSize.CompareTo(y.FileSize)); // 升序
        // 这是以前对observablecollection设计的：
        // var list = Mangas.OrderByDescending(func).ToList();
        //OrderBy方法不会修改源数据，返回的值是与源挂钩的，源清零，返回值也清零

        //Mangas.Clear();

        //foreach (var book in list)
        //{
        //    Mangas.Add(book);
        //}
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="name"></param>
    /// <param name="chapteramount"></param>
    /// <param name="filesize"></param>
    /// <exception cref="NotImplementedException"></exception>
    [Obsolete("未完成")]
    public void Filter(string name, int chapteramount, long filesize)
    {
        var query = Mangas.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(m => m.Name.Contains(name));

        if (chapteramount != 0)
        {
            Func<Manga, bool> amount = chapteramount switch
            {
                0 => manga => true, //无章节数限制
                1 => manga => manga.Chapters.Count == 1, //单章节
                2 => manga => manga.ChapterAmount >= 2, //多章节
                _ => throw new NotImplementedException(),
            };

            query = query.Where(amount);
        }

        if (filesize != 0)
        {
            query = query.Where(m => m.FileSize >= filesize);
        }

        FilteredMangas = [.. query];
    }

    /// <summary>
    /// 按范围显示
    /// </summary>
    /// <param name="startindex"></param>
    /// <param name="count"></param>
    public void Display(int startindex, int count)
    {
        // 限定范围
        startindex = Math.Max(0, startindex); // 保证非负
        startindex = Math.Min(startindex, Mangas.Count); // 保证不超过集合长度
        int end = Math.Min(startindex + count, Mangas.Count); // 保证 end不越界

        DisplayMangas.Clear();
        var mangas = Mangas[startindex..end];
        foreach (var manga in mangas)
            DisplayMangas.Add(manga);
    }

    #region 筛选 / 显示

    /// <summary>每页固定条数</summary>
    public const int PageSize = 20;

    private List<Manga> _filtered = [];
    private bool _isFiltered = false;

    /// <summary>当前有效源：筛选后 or 全量</summary>
    private IReadOnlyList<Manga> CurrentSource => _isFiltered ? _filtered : Mangas;

    /// <summary>当前有效源总条数</summary>
    public int CurrentCount => CurrentSource.Count;

    /// <summary>分页模式下的总页数（至少 1）</summary>
    public int PageCount => CurrentCount == 0 ? 1 : (CurrentCount + PageSize - 1) / PageSize;

    /// <summary>
    /// 应用筛选。只更新内部 _filtered，不碰 DisplayMangas。
    /// 调用方随后应调 RefreshDisplay。
    /// </summary>
    public void ApplyFilter(MangaFilterOptions o)
    {
        IEnumerable<Manga> q = Mangas;

        if (o.FilterByName && !string.IsNullOrWhiteSpace(o.NameKeyword))
        {
            var kw = o.NameKeyword.Trim();
            q = q.Where(m =>
                m.Name.Contains(kw, StringComparison.OrdinalIgnoreCase)
                || m.TranslatedName.Contains(kw, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (o.FilterByFileSize)
        {
            q = q.Where(m =>
                m.FileSize >= o.MinFileSize && (o.MaxFileSize <= 0 || m.FileSize <= o.MaxFileSize)
            );
        }

        if (o.FileTypes.Count > 0)
        {
            bool wantFolder = o.FileTypes.Contains("__folder__");
            var exts = o
                .FileTypes.Where(t => t != "__folder__")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            q = q.Where(m =>
                (wantFolder && string.IsNullOrEmpty(m.Type))
                || (!string.IsNullOrEmpty(m.Type) && exts.Contains(m.Type))
            );
        }

        if (o.FilterByTag && !string.IsNullOrWhiteSpace(o.TagKeyword))
        {
            var kws = o.TagKeyword.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries
            );
            q = q.Where(m =>
                kws.Any(k => m.Tags.Any(t => t.Contains(k, StringComparison.OrdinalIgnoreCase)))
            );
        }

        _filtered = [.. q];
        _isFiltered = true;
    }

    /// <summary>清除筛选，回到全量</summary>
    public void ClearFilter()
    {
        _isFiltered = false;
        _filtered.Clear();
    }

    /// <summary>
    /// 刷新 DisplayMangas。
    /// paged=false：显示全部；paged=true：显示第 page 页（page 从 1 开始）。
    /// </summary>
    public void RefreshDisplay(bool paged, int page)
    {
        var src = CurrentSource;

        int start = 0,
            count = src.Count;
        if (paged)
        {
            page = Math.Max(1, page);
            start = (page - 1) * PageSize;
            if (start >= src.Count)
                start = Math.Max(0, (PageCount - 1) * PageSize);
            count = Math.Min(PageSize, src.Count - start);
        }

        DisplayMangas.Clear();
        for (int i = start; i < start + count; i++)
            DisplayMangas.Add(src[i]);
    }

    #endregion
}
