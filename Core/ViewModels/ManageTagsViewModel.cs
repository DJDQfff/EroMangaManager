using CommunityToolkit.Mvvm.Input;

namespace EroMangaManager.Core.ViewModels;

[Obsolete]
public partial class ManageTagsViewModel : ObservableObject
{
    [ObservableProperty]
    private string displayedCategory;

    [ObservableProperty]
    private ObservableCollection<string> displayedTags;

    private readonly Dictionary<string, ObservableCollection<string>> keyValuePairs;

    /// <summary>
    ///
    /// </summary>
    public ManageTagsViewModel()
    {
        var a = DatabaseController.TagCategory_QueryAll();

        keyValuePairs = a.ToDictionary(x => x.Key, y => new ObservableCollection<string>(y.Value));
        Categorys = new(a.Keys);
    }

    /// <summary>
    ///
    /// </summary>
    public event Action CategorysChanged;

    /// <summary>
    ///
    /// </summary>
    public ObservableCollection<string> Categorys { get; }

    /// <summary>
    /// 已分类的tag
    /// </summary>
    public IEnumerable<string> Tags
    {
        get
        {
            var list = new List<string>();

            foreach (var a in keyValuePairs)
            {
                list.AddRange(a.Value);
            }
            return list.Distinct();
        }
    }

    /// <summary>
    /// 未分类的tag
    /// </summary>
    public ObservableCollection<string> UnCategoryTags { set; get; } = [];

    /// <summary>
    ///
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public ObservableCollection<string> this[string category] => keyValuePairs[category];

    partial void OnDisplayedCategoryChanged(string value)
    {
        if (value is not null)
        {
            DisplayedTags = this[value];
        }
    }

    /// <summary>
    /// 添加新分类
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    [RelayCommand]
    public void AddCategory(string category)
    {
        if (!string.IsNullOrWhiteSpace(category))
        {
            keyValuePairs.Add(category, new ObservableCollection<string>());
            Categorys.Add(category);
            OnPropertyChanged(nameof(Categorys));
            CategorysChanged?.Invoke();
        }
    }

    /// <summary>
    /// 传入tags，先过滤已分类的tag，剩下的全部挪到未分类里面
    /// </summary>
    /// <param name="tags"></param>
    public void AddUnCategoryTags(IEnumerable<string> tags)
    {
        var a = tags.Except(Tags).Distinct();

        foreach (var tag in a)
        {
            UnCategoryTags.Add(tag);
        }
    }

    /// <summary>
    /// 移除某一分类
    /// </summary>
    /// <param name="category"></param>
    [RelayCommand]
    public void DeleteCategory(string category)
    {

        foreach (var a in keyValuePairs[category])
        {
            UnCategoryTags.Add(a);
        }

        Categorys.Remove(category);
        keyValuePairs.Remove(category);
        OnPropertyChanged(nameof(Categorys));
        CategorysChanged?.Invoke();
    }

    /// <summary>
    /// 改变某一tag的分类
    /// </summary>
    /// <param name="oldcategory"></param>
    /// <param name="newcategory"></param>
    /// <param name="tags"></param>
    public void TagChangeCategory(string oldcategory, string newcategory, IList<string> tags)
    {
        foreach (var tag in tags)
        {
            keyValuePairs[oldcategory].Remove(tag);
            keyValuePairs[newcategory].Add(tag);
        }
    }
}