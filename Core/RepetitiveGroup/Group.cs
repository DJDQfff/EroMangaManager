using System.Linq;          // IGrouping
using System.Collections;   // IEnumerator / IEnumerable
namespace Core.RepetitiveGroup;

/// <summary>
/// 按某条件分好的组
/// </summary>
/// <typeparam name="TKey">重复分组依据</typeparam>
/// <typeparam name="TElement">重复项</typeparam>
public class Group<TKey, TElement> : IGrouping<TKey, TElement>, IDisposable
{
    /// <summary>
    /// 重复项分组依据，
    /// </summary>
    public TKey Key { set; get; } = default!; // => files.Key;

    /// <summary>
    /// 重复项可观察集合
    /// </summary>
    public ObservableCollection<TElement> Collections { get; } = [];

    /// <summary>
    /// 初始化数据
    /// </summary>
    /// <param name="_files"></param>
    public void Initial(IGrouping<TKey, TElement> _files)
    {
        Key = _files.Key;
        foreach (var file in _files)
        {
            Collections.Add(file);
        }
    }

    public void Initial(TKey key)
    {
        Key = key;
    }

    public void AddElement(TElement element)
    {
        Collections.Add(element);
    }

    public void AddElement(IEnumerable<TElement> elements)
    {
        foreach (var element in elements)
        {
            Collections.Add(element);
        }
    }

    /// <summary>
    /// 移除重复项中的某一个
    /// </summary>
    /// <returns>剩下的项的个数</returns>
    public int TryRemoveItem(TElement element)
    {
        _ = Collections.Remove(element);

        return Collections.Count;
    }

    /// <summary>
    /// 迭代器
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TElement> GetEnumerator() => Collections.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 清空资源
    /// </summary>
    public void Dispose()
    {
        Collections.Clear();
        GC.SuppressFinalize(this);
    }

    internal void Initial()
    {
        throw new NotImplementedException();
    }
}
