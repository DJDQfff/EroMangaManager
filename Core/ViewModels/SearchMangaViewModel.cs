using System.Runtime.CompilerServices;

namespace Core.ViewModels;

/// <summary>
/// 搜索与筛选专用 ViewModel，依赖 ObservableCollectionVM 提供数据源
/// </summary>
public partial class SearchMangaViewModel(ObservableCollectionVM collectionVM) : ObservableObject
{
    #region 搜索状态与结果 (支持 UI 绑定)

    /// <summary>
    /// 对批量搜索结果执行事件
    /// </summary>
    public event Action<IEnumerable<Manga>>? ResultNewAddMulti;

    /// <summary>
    /// 对单个搜索结果执行事件
    /// </summary>
    public event Func<Manga, Task>? ResultNewAddSingle;

    /// <summary>
    /// 搜索文本 (使用 ObservableProperty 支持 UI 双向绑定)
    /// </summary>
    [ObservableProperty]
    public partial string RequiredText { get; set; } = "";

    /// <summary>
    /// 搜索必须包含的 tag
    /// </summary>
    public ObservableCollection<string> RequiredTags { get; } = [];

    /// <summary>
    /// 搜索可选 tag（用于 UI 提示/补全）
    /// </summary>
    public ObservableCollection<string> AvailableTags { get; } = [];

    /// <summary>
    /// 搜索结果集合
    /// </summary>
    public ObservableCollection<Manga> ResultMangas { get; } = [];

    #endregion

    #region 搜索与筛选方法

    /// <summary>
    /// 异步流式过滤方法，支持取消令牌，避免大集合阻塞 UI 线程
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

    /// <summary>
    /// 执行搜索内容，更新结果集合
    /// </summary>
    public async Task SearchAsync()
    {
        var result = FilterMangas(RequiredText, RequiredTags);

        ResultMangas.Clear();
        foreach (var manga in result)
        {
            if (ResultNewAddSingle != null)
            {
                await ResultNewAddSingle.Invoke(manga);
            }
            await Task.Yield(); // 让出线程，避免 UI 卡顿
            ResultMangas.Add(manga);
        }

        // 触发批量添加事件（兼容旧版逻辑）
        ResultNewAddMulti?.Invoke(ResultMangas);
    }

    /// <summary>
    /// 同步搜索（如果不需要异步逐条添加，可直接使用此方法）
    /// </summary>
    public void Search()
    {
        var result = FilterMangas(RequiredText, RequiredTags).ToList();
        ResultMangas.Clear();
        foreach (var manga in result)
        {
            ResultMangas.Add(manga);
        }
        ResultNewAddMulti?.Invoke(ResultMangas);
    }

    /// <summary>
    /// 筛选可选 tag（用于搜索框补全/下拉建议）
    /// </summary>
    public void FiltTags(string query)
    {
        AvailableTags.Clear();
        // 数据源来自注入的 ObservableCollectionVM
        foreach (var tag in collectionVM.AllTags.Except(RequiredTags))
        {
            if (tag.Contains(query))
                AvailableTags.Add(tag);
        }
    }

    /// <summary>
    /// 底层过滤逻辑（私有）
    /// </summary>
    private IEnumerable<Manga> FilterMangas(string? name, IEnumerable<string> tags)
    {
        // 基础校验：无搜索条件时返回空，避免无条件全量加载
        if (string.IsNullOrWhiteSpace(name) && (tags == null || !tags.Any()))
        {
            return [];
        }

        // 数据源来自注入的 ObservableCollectionVM
        return collectionVM
            .MangasGroups.SelectMany(x => x.Mangas)
            .Where(x => x.Name.Contains(name?.Trim() ?? string.Empty))
            .Where(x => tags == null || !tags.Any() || tags.All(y => x.Tags.Contains(y)));
    }

    #endregion
}
