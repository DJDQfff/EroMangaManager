using System.Diagnostics.CodeAnalysis;

namespace Core.ViewModels;

/// <summary>
/// 本子组
/// </summary>
public partial class MangasGroup : ObservableObject
{
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
    /// 为过滤国能设置，未完成
    /// </summary>
    [Obsolete("未完成")]
    public List<Manga> FilteredMangas { private set; get; } = [];

    /// <summary>
    /// 展示在UI中的Manga
    /// </summary>
    public ObservableCollection<Manga> DisplayMangas { get; set; } = [];

    /// <summary>
    /// 所有标签（已简单去重）
    /// </summary>
    public IEnumerable<string> AllTags => Mangas.SelectMany(x => x.Tags).Distinct();

    /// <summary>
    /// 不当文件夹用，所以不指定文件夹路径
    /// </summary>
    public MangasGroup() { }

    /// <summary>
    ///
    /// </summary>
    public string Guid { get; set; } = string.Empty;

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

    [ObservableProperty]
    public partial int Count { get; set; }

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
        // TODO ，看看调用层次是否需要返回值
        var a = Mangas.Remove(mangaBook);
        var b = DisplayMangas.Remove(mangaBook);
        Count = Mangas.Count;

        if (a == b)
        {
            return a;
        }
        else
        {
            return !a;
        }
    }
}
