namespace Core.RepetitiveGroup;

/// <summary>
/// 重复项组合ViewModel
/// </summary>
/// <typeparam name="TKey">重复项分组依据</typeparam>
/// <typeparam name="TElement">重复项 Model</typeparam>
/// <typeparam name="TGroup">重复项组合</typeparam>
public class GroupsViewModel<TKey, TElement, TGroup>
    where TGroup : Group<TKey , TElement>, new()
{
    /// <summary>
    /// 重复项组合的集合
    /// </summary>
    public ObservableCollection<TGroup> RepeatPairs { set; get; } = [];

    /// <summary>
    /// 组合个数
    /// </summary>
    public int Count => RepeatPairs.Count;

    /// <summary>
    /// 获取所有元素项
    /// </summary>
    public IEnumerable<TElement> AllElements
    {
        get
        {
            var list = new List<TElement>();
            foreach (var item in RepeatPairs)
            {
                var em = item.Collections;
                list.AddRange(em);
            }
            return list;
        }
    }

    /// <summary>
    /// 删除一个项，在集合中检测删除
    /// </summary>
    /// <param name="elment"></param>
    public void DeleteStorageFileInRootObservable (TElement elment)
    {
        // 曾出现bug： RepeatPairs是一个ObservableCollection，增减元素经常交替发生，使用index会出问题

        for (int index = Count - 1 ; index >= 0 ; index--)
        {
            var group = RepeatPairs[index];

            var count = group.TryRemoveItem(elment);
            // 没有重复项后，会自动从集合中移除此集合
            if (count == 0)
            {
                RepeatPairs.Remove(group);

                //RepeatPairs.RemoveAt(index);
            }
        }
    }
}