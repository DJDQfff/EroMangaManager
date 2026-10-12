using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Core.Services;
using iText.Commons.Internal.Runtime;

namespace Core.ViewModels;

/// <summary>
/// 搜索 / 筛选 / 分页 统一 ViewModel。
///
/// 设计目的：
/// 1. 数据源通过 Func 提供，兼容"固定源"（MangasGroup）和"实时源"（全局集合）。
/// 2. 同时服务两类页面：
///    - Bookcase：单组数据，带分页 + 大小 / 类型筛选，无条件时显示全部。
///    - GlobalSearch：全局数据，不分页，无条件时返回空。
/// 3. 提供两套筛选入口，各自用途不同：
///    - ApplyFilter / ApplyFilterAndRefresh / RefreshDisplay：读 VM 上的条件属性，
///      写内部 _filtered，供 UI 主流程使用（Bookcase / GlobalSearch 的搜索框、筛选面板）。
///    - FilterMangasAsync / FilterMangas：条件由参数传入，不读 VM 属性，不写 _filtered，
///      供外部调用方做流式过滤（如 ASPNETCoreServer 的 /mangas/with_tag/{tag}）。
/// 4. 分页只影响 RefreshDisplay 的切片，不影响 ApplyFilter 的过滤结果。
/// </summary>
public partial class SearchFilterViewModel(
    Func<IReadOnlyList<Manga>> sourceProvider,
    CoverSetter coverSetter
) : ObservableObject
{
    /// <summary>当前源（每次访问重新取，保证实时性）</summary>
    public IReadOnlyList<Manga> Source => sourceProvider();

    #region 筛选条件（UI 直接绑这里）

    /// <summary>名称关键字（匹配 Name / TranslatedName）</summary>
    [ObservableProperty]
    public partial string NameKeyword { get; set; } = "";

    /// <summary>必须包含的标签（全部命中）</summary>
    public ObservableCollection<string> RequiredTags { get; } = [];

    /// <summary>是否启用大小筛选</summary>
    [ObservableProperty]
    public partial bool FilterByFileSize { get; set; }

    [ObservableProperty]
    public partial long MinFileSize { get; set; }

    /// <summary>是否启用章节数筛选</summary>
    [ObservableProperty]
    public partial bool FilterByChapterAmount { get; set; }

    [ObservableProperty]
    public partial int MinChapterAmount { get; set; } = 1;

    /// <summary>0 表示不限</summary>
    [ObservableProperty]
    public partial int MaxChapterAmount { get; set; }

    /// <summary>0 表示不限</summary>
    [ObservableProperty]
    public partial long MaxFileSize { get; set; }

    /// <summary>文件类型（"__folder__" 表示文件夹；永远至少一个）</summary>
    public ObservableCollection<string> FileTypes { get; } = ["__folder__", ".zip"];

    /// <summary>
    /// 无条件时是否显示全部。
    /// Bookcase = true；GlobalSearch = false（无条件返回空）。
    /// </summary>
    [ObservableProperty]
    public partial bool ShowAllWhenNoCondition { get; set; } = true;

    #endregion
    #region 筛选条件（UI 直接绑这里）


    /// <summary>是否启用章节名筛选</summary>
    [ObservableProperty]
    public partial bool FilterByChapterName { get; set; }

    /// <summary>章节名关键字（任一 Chapter.Chaptername 包含即命中）</summary>
    [ObservableProperty]
    public partial string ChapterNameKeyword { get; set; } = "";

    #endregion
    #region 显示 / 分页

    [ObservableProperty]
    public partial bool IsPaged { get; set; } = true;

    [ObservableProperty]
    public partial int PageSize { get; set; } = 20;

    /// <summary>当前页（从 1 开始）。NumberBox 双向绑这个。</summary>
    [ObservableProperty]
    public partial int CurrentPage { get; set; } = 1;

    /// <summary>总页数（至少 1）。RefreshDisplay 里赋值，UI 可绑。</summary>
    [ObservableProperty]
    public partial int PageCount { get; set; } = 1;

    /// <summary>当前有效源的总条数（筛选后）</summary>
    public int FilteredCount => CurrentSource.Count;

    /// <summary>UI 实际绑定</summary>
    public ObservableCollection<Manga> DisplayMangas { get; } = [];

    /// <summary>可用标签（补全用，每次查询；量大再缓存）</summary>
    public IReadOnlyList<string> AvailableTags =>
        [
            .. Source
                .SelectMany(x => x.Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t),
        ];

    private List<Manga> _filtered = [];
    private bool _isFiltered = false;

    /// <summary>当前有效源：筛选后 or 全量</summary>
    private IReadOnlyList<Manga> CurrentSource => _isFiltered ? _filtered : Source;

    // =====================================================================
    // 以下为「供 UI 调用」的入口：
    // 读 VM 上的条件属性，写内部 _filtered / DisplayMangas。
    // Bookcase / GlobalSearch 的主流程走这里。
    // =====================================================================

    /// <summary>
    /// 应用当前条件。只更新内部 _filtered，不碰 DisplayMangas。
    /// 调用方随后应调 RefreshDisplay（或 ApplyFilterAndRefresh）。
    ///
    /// 注意：类型筛选（FileTypes）永远生效，至少一个。
    /// GlobalSearch 若不做类型筛选，应把 FileTypes 清空。
    /// </summary>
    public void ApplyFilter()
    {
        var src = Source;

        bool fileTypesIsDefault =
            FileTypes.Count == 2 && FileTypes.Contains("__folder__") && FileTypes.Contains(".zip");

        bool noCondition =
            string.IsNullOrWhiteSpace(NameKeyword)
            && RequiredTags.Count == 0
            && !FilterByFileSize
            && !FilterByChapterAmount
            && !FilterByChapterName
            && fileTypesIsDefault;
        if (noCondition)
        {
            _filtered = ShowAllWhenNoCondition ? [.. src] : [];
            _isFiltered = true;
            return;
        }

        IEnumerable<Manga> q = src;

        if (!string.IsNullOrWhiteSpace(NameKeyword))
        {
            var kw = NameKeyword.Trim();
            q = q.Where(m =>
                m.Name.Contains(kw, StringComparison.OrdinalIgnoreCase)
                || m.TranslatedName.Contains(kw, StringComparison.OrdinalIgnoreCase)
            );
        }
        if (FilterByChapterName && !string.IsNullOrWhiteSpace(ChapterNameKeyword))
        {
            var kw = ChapterNameKeyword.Trim();
            q = q.Where(m =>
                m.Chapters.Any(c => c.Chaptername.Contains(kw, StringComparison.OrdinalIgnoreCase))
            );
        }
        if (RequiredTags.Count > 0)
        {
            q = q.Where(m =>
                RequiredTags.All(tag =>
                    m.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))
                )
            );
        }

        if (FilterByFileSize)
        {
            q = q.Where(m =>
                m.FileSize >= MinFileSize && (MaxFileSize <= 0 || m.FileSize <= MaxFileSize)
            );
        }
        if (FilterByChapterAmount)
        {
            int min = Math.Max(1, MinChapterAmount);
            q = q.Where(m =>
                m.Chapters.Count >= min
                && (MaxChapterAmount <= 0 || m.Chapters.Count <= MaxChapterAmount)
            );
        } // 类型永远生效，至少一个
        if (FileTypes.Count > 0)
        {
            bool wantFolder = FileTypes.Contains("__folder__");
            var exts = FileTypes
                .Where(t => t != "__folder__")
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            q = q.Where(m =>
                (wantFolder && string.IsNullOrEmpty(m.Type))
                || (!string.IsNullOrEmpty(m.Type) && exts.Contains(m.Type))
            );
        }

        _filtered = [.. q];
        _isFiltered = true;
    }

    /// <summary>清空条件并回到全量</summary>
    public void ClearFilter()
    {
        NameKeyword = "";
        FilterByChapterName = false;
        ChapterNameKeyword = "";
        RequiredTags.Clear();
        FilterByFileSize = false;
        MinFileSize = 0;
        MaxFileSize = 0;
        FilterByChapterAmount = false;
        MinChapterAmount = 1;
        MaxChapterAmount = 0;
        FileTypes.Clear();
        FileTypes.Add("__folder__");
        FileTypes.Add(".zip");

        _isFiltered = false;
        _filtered.Clear();
        CurrentPage = 1;
    }

    /// <summary>重算 PageCount 和 DisplayMangas（读 CurrentPage）</summary>
    public void RefreshDisplay()
    {
        var src = CurrentSource;

        if (IsPaged)
        {
            PageCount = src.Count == 0 ? 1 : (src.Count + PageSize - 1) / PageSize;

            CurrentPage = Math.Clamp(CurrentPage, 1, PageCount);
            int start = (CurrentPage - 1) * PageSize;
            int count = Math.Min(PageSize, src.Count - start);

            DisplayMangas.Clear();
            for (int i = start; i < start + count; i++)
                DisplayMangas.Add(src[i]);
        }
        else
        {
            PageCount = 1;
            CurrentPage = 1;

            DisplayMangas.Clear();
            foreach (var m in src)
                DisplayMangas.Add(m);
        }

        OnPropertyChanged(nameof(FilteredCount));
    }

    /// <summary>应用筛选并刷新（常用组合）</summary>
    public void ApplyFilterAndRefresh()
    {
        ApplyFilter();
        CurrentPage = 1;
        RefreshDisplay();
    }

    /// <summary>清空筛选并刷新</summary>
    public void ClearFilterAndRefresh()
    {
        ClearFilter();
        RefreshDisplay();
    }

    /// <summary>翻页</summary>
    public void GoToPage(int page)
    {
        CurrentPage = Math.Clamp(page, 1, Math.Max(1, PageCount));
        RefreshDisplay();
    }

    /// <summary>
    /// 应用筛选后，逐条把结果交给 ResultNewAddSingle（用于封面逐张加载），
    /// 最后触发 ResultNewAddMulti。
    /// </summary>
    public async Task SearchAsync()
    {
        ApplyFilter();
        CurrentPage = 1;
        RefreshDisplay();

        foreach (var manga in DisplayMangas.ToArray())
        {
            await coverSetter.SingleLoadWork(manga, true, true);
            await Task.Yield();
        }

        //await coverSetter.MultiLoadWork(DisplayMangas, true, true);
    }

    /// <summary>同步版本</summary>
    public async Task Search()
    {
        ApplyFilterAndRefresh();
        await coverSetter.MultiLoadWork(DisplayMangas, true, true);
    }

    /// <summary>标签补全过滤（原 FiltTags）。AvailableTags 是只读计算属性，结果写入 target。</summary>
    public void FiltTags(string query, ObservableCollection<string> target)
    {
        target.Clear();
        foreach (var tag in AvailableTags.Except(RequiredTags))
        {
            if (
                string.IsNullOrEmpty(query)
                || tag.Contains(query, StringComparison.OrdinalIgnoreCase)
            )
                target.Add(tag);
        }
    }

    #endregion

    // =====================================================================
    // 以下为「供外部调用」的入口：
    // 条件由参数传入，不读 VM 属性，不写 _filtered / DisplayMangas。
    // 例如 ASPNETCoreServer 的流式接口。
    // =====================================================================

    #region 外部入口：流式过滤
    /// <summary>
    /// 同步过滤逻辑。条件由参数传入。
    /// 无条件时返回空（保持原 SearchMangaViewModel 的语义）。
    /// </summary>
    private IEnumerable<Manga> FilterMangas(string? name, IEnumerable<string> tags)
    {
        if (string.IsNullOrWhiteSpace(name) && (tags is null || !tags.Any()))
            return [];

        IEnumerable<Manga> q = Source;

        if (!string.IsNullOrWhiteSpace(name))
        {
            var kw = name.Trim();
            q = q.Where(m =>
                m.Name.Contains(kw, StringComparison.OrdinalIgnoreCase)
                || m.TranslatedName.Contains(kw, StringComparison.OrdinalIgnoreCase)
            );
        }

        if (tags is not null && tags.Any())
        {
            q = q.Where(m =>
                tags.All(y => m.Tags.Any(t => t.Equals(y, StringComparison.OrdinalIgnoreCase)))
            );
        }

        return q;
    }

    /// <summary>
    /// 异步流式过滤，支持取消。条件由参数传入，不读 VM 上的属性。
    /// 仅为同步过滤的异步封装
    /// </summary>
    public async IAsyncEnumerable<Manga> FilterMangasAsync(
        string? name,
        IEnumerable<string> tags,
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        foreach (var manga in FilterMangas(name, tags))
        {
            ct.ThrowIfCancellationRequested();
            await Task.Yield();
            yield return manga;
        }
    }

    #endregion
}
