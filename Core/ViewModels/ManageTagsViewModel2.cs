using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using Database;

namespace Core.ViewModels;

public partial class ManageTagsViewModel2
{
    readonly DatabaseController databaseController;

    /// <summary>
    /// 从数据库读取category以初始化
    /// </summary>
    public ManageTagsViewModel2(DatabaseController _databaseController)
    {
        databaseController = _databaseController;
        var a = databaseController.TagCategoryArray();
        CategoryTags = new(a);
    }

    /// <summary>
    /// 分类已存在时触发此事件
    /// </summary>
    public event Action<string>? CategoryAlreadyExists;

    /// <summary>
    /// 分类改变事件
    /// </summary>
    public event Action? CategorysChanged;

    /// <summary>
    /// 已分类的tagcategory
    /// </summary>
    public ObservableCollection<TagCategory> CategoryTags { get; }

    /// <summary>
    /// 未分类的tag
    /// </summary>
    public ObservableCollection<string> ImCategoryedTags { get; } = [];

    /// <summary>
    /// 选中的tagcategory，其实可以view的字段，但是有bug，所以单独弄了一个
    /// </summary>
    public TagCategory? SelectedTagCategory { set; get; }

    /// <summary>
    /// 已分类的tag
    /// </summary>
    public IEnumerable<string> Tags
    {
        get
        {
            var list = new List<string>();

            foreach (var a in CategoryTags)
            {
                list.AddRange(a.Tags);
            }
            return list.Distinct();
        }
    }

    /// <summary>
    /// 添加新分类
    /// </summary>
    /// <param name="category"></param>
    /// <returns></returns>
    public async Task<TagCategory?> AddCategory(string category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category); // ✅ 快速失败，带明确消息
        if (CategoryTags.FirstOrDefault(x => x.CategoryName == category) is null)
        {
            var tagCategory = await databaseController.TagCategory_AddCategorySingle(category);

            CategoryTags.Add(tagCategory);

            CategorysChanged?.Invoke();
            return tagCategory;
        }
        else
        {
            CategoryAlreadyExists?.Invoke(category);
            return null;
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
            ImCategoryedTags.Add(tag);
        }
    }

    /// <summary>
    /// 移除某一分类
    /// </summary>
    /// <param name="category"></param>
    [RelayCommand]
    public async Task DeleteCategory(string category)
    {
        var tagCategory = CategoryTags.FirstOrDefault(x => x.CategoryName == category);
        if (tagCategory != null)
        {
            var tags = tagCategory.Tags;
            CategoryTags.Remove(tagCategory);
            await databaseController.TagCategory_RemoveCategory(tagCategory);
            foreach (var tag in tags)
            {
                ImCategoryedTags.Add(tag);
            }
            CategorysChanged?.Invoke();
        }
    }

    /// <summary>
    /// 改变某一tag的分类
    /// </summary>
    /// <param name="oldcategory"></param>
    /// <param name="newcategory"></param>
    /// <param name="tags"></param>
    public void TagChangeCategory(
        TagCategory oldcategory,
        TagCategory newcategory,
        IList<string> tags
    )
    {
        if (oldcategory is null)
        {
            foreach (var tag in tags)
            {
                ImCategoryedTags.Remove(tag);
                newcategory.Tags.Add(tag);
            }
        }
        else
        {
            foreach (var tag in tags)
            {
                oldcategory.Tags.Remove(tag);
                newcategory.Tags.Add(tag);
            }
        }
    }
}
