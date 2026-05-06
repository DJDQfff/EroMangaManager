namespace EroMangaManager.Core.ViewModels;

/// <summary>
/// 本子组
/// </summary>
public partial class MangasGroup : ObservableObject
{
    /// <summary>
    /// 文件夹路径，一开始是作为文件夹设计的，后来不作为文件夹，仅作为本子统一集合
    /// </summary>
    public string FolderPath { get; set; }

    ///// <summary>
    ///// 更新状态，表示是否再后台更新
    ///// </summary>
    [ObservableProperty]
    private MangasGroupUpdateState updateState = MangasGroupUpdateState.Ready;

    /// <summary>
    /// 本子集合
    /// </summary>
    public ObservableCollection<Manga> Mangas { get; set; } = [];

    /// <summary>
    /// 所有标签
    /// </summary>
    public IEnumerable<string> AllTags
    {
        get
        {
            List<string> tags = [];
            foreach (var x in Mangas)
            {
                tags.AddRange(x.Tags);
            }
            return tags;
        }
    }

    /// <summary>
    /// 不当文件夹用，所以不指定文件夹路径
    /// </summary>
    public MangasGroup()
    { }
    public string Guid { get;  set; }
    /// <summary>
    /// 当文件夹用，需要指定文件夹路径
    /// </summary>
    /// <param name="storageFolderPath"></param>
    public MangasGroup(string storageFolderPath)
    {
        
        FolderPath = storageFolderPath;
    }

    /// <summary>
    /// 对内部漫画进行排序
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="func"></param>
    public void SortMangas<TKey>(Func<Manga, TKey> func)
    {
        var list = Mangas.OrderByDescending(func).ToList();        //OrderBy方法不会修改源数据，返回的值是与源挂钩的，源清零，返回值也清零

        Mangas.Clear();

        foreach (var book in list)
        {
            Mangas.Add(book);
        }
    }

    /// <summary>
    /// 移除一个本子
    /// </summary>
    /// <param name="mangaBook"></param>
    public bool RemoveManga(Manga mangaBook)
    {
        return Mangas.Remove(mangaBook);
    }
}