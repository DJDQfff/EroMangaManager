namespace EroMangaManager.Core.ViewModels;

/// <summary>
/// 本子标签管理VM
/// </summary>
/// <remarks>
/// 搜索ViewModel
/// </remarks>
public partial class SearchMangaViewModel : ObservableObject
{
    public event Action<IEnumerable<Manga>> ResultNewAdd;
    /// <summary>
    /// 所有要查重的manga集合
    /// </summary>
    public List<Manga> Sources { set; get; } = [];

    /// <summary>
    /// 选中项
    /// </summary>
    public ObservableCollection<string> RequiredTags { get; set; } = [];

    [ObservableProperty]
    private string requiredText = "";

    /// <summary>
    /// 对外公开的所有项
    /// </summary>
    public List<string> AllTags { set; get; }

    /// <summary>
    /// 可能需要的tag
    /// </summary>
    public ObservableCollection<string> AlailableTags { get; } = [];

    /// <summary>
    ///
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public void FiltTags(string query)
    {
        AlailableTags.Clear();

        foreach (var x in AllTags.Except(RequiredTags))
        {
            if (x.Contains(query))
            {
                AlailableTags.Add(x);
            }
        }
        ;
    }

    /// <summary>
    /// 按搜索条件筛选出来的本子
    /// </summary>
    public ObservableCollection<Manga> ResultMangas { get; } = [];

    /// <summary>
    /// 按条件查找manga
    /// </summary>
    public void Search()
    {
        ResultMangas.Clear();

        // TODO 有点小bug，

        var a = Sources
        .Where(x => x.MangaName.Contains(RequiredText.Trim())) //manganame中包含指定字符串
        .Where(x => RequiredTags.All(y => x.Tags.Contains(y))); //

        //var a = Sources
        //.Where(x => RequiredText.Split(' ').Any(y => x.FileDisplayName.Contains(y)));
        //if (RequiredTags.Count != 0)
        //{
        //    a = a.Where(x => RequiredTags.Any(y => x.FileDisplayName.Contains(y)));
        //}
        //a = a.OrderBy(x => x.MangaName);

        foreach (var x in a)
        {
            ResultMangas.Add(x);
        }
        ResultNewAdd?.Invoke(a);

        //}
    }
}