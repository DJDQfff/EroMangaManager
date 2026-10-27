namespace Core.CollectionFindRepeat;

/// <summary>
/// 对一个较长字符串的集合，查找重复出现的短字符串
/// </summary>
public class StringCollection<TSource, TArrayItem>
{
    /// <summary>
    /// 重复数组最短长度
    /// </summary>
    public int MinItemLength = 2;

    /// <summary>
    /// 重复数组最少重复次数
    /// </summary>
    public int MinOccurTimes = 2;

    /// <summary>
    /// 字符串集合
    /// </summary>
    public IEnumerable<TSource> Sources { get; set; } = [];

    public Func<TSource, TArrayItem[]> Action { set; get; } = null!;
    public List<RepeatItem> RepeatItemsList { get; } = [];

    /// <summary>
    /// 增字查找
    /// </summary>
    public void Run()
    {
        var checkList = Sources
            .Select(x => new CheckTarget(x) { ParserArray = Action(x) })
            .OrderBy(x => x.ParserArray.Length)
            .ToList();

        var repeatitems = new List<string>();
        for (int index = 0; index < checkList.Count; index++)
        {
            var currentCheckItem = checkList[index];
            //var behindFdj = checkList.GetRange(index + 1, checkList.Count - index);
            var maxLength = currentCheckItem.ParserArray.Length;
            for (int start = 0; start < maxLength; start++)
            {
                for (int length = MinItemLength; start + length <= maxLength; length++)
                {
                    var item = currentCheckItem.ParserArray[start..(length + start)]; // 这是以前的.Skip(start).Take(length);
                    CountBehind(checkList, index, item);
                }
            }
        }
    }

    /// <summary>
    /// 这个是我让ai生成的，没有测试过。下面注释掉的才是自己写的。
    /// </summary>
    public void Run2()
    {
        var checkTargets = Sources
            .Select(x => new CheckTarget(x) { ParserArray = Action(x) })
            .OrderBy(x => x.ParserArray.Length)
            .ToArray();

        for (int index = 0; index < checkTargets.Length; index++)
        {
            var currentCheck = checkTargets[index];
            var currentCheckArrayLength = currentCheck.ParserArray.Length;

            // 调用提取出来的方法，如果找到则直接跳出外层循环
            if (FindMatch(currentCheck, currentCheckArrayLength, checkTargets, index))
            {
                break;
            }
        }
    }

    //public void Run2()
    //{
    //    var checkTargets = Sources
    //        .Select(x => new CheckTarget(x) { ParserArray = Action(x) })
    //       .OrderBy(x => x.ParserArray.Length)
    //       .ToArray();

    //    for (int index = 0; index < checkTargets.Length; index++)
    //    {
    //        var currentCheck = checkTargets[index];
    //        //var behindFdj = checkList.GetRange(index + 1, checkList.Count - index);
    //        var currentCheckArrayLength = currentCheck.ParserArray.Length;
    //        for (int start = 0; start < currentCheckArrayLength; start++)
    //        {
    //            for (
    //                int length = currentCheckArrayLength;
    //                MinItemLength <= length && start + length <= currentCheckArrayLength;
    //                length--
    //            )
    //            {
    //                var item = currentCheck.ParserArray[start..(length - start)];//TODO 要检查索引是不是对的/*.Skip(start).Take(length);*/

    //                if (CountBehind(checkTargets, index, item))
    //                {
    //                    goto JDKJFK;
    //                }
    //            }
    //        JDKJFK:
    //            break;
    //        }
    //    }
    //}

    // 提取出的查找逻辑
    private bool FindMatch(
        CheckTarget currentCheck,
        int arrayLength,
        CheckTarget[] targets,
        int currentIndex
    )
    {
        for (int start = 0; start < arrayLength; start++)
        {
            for (
                int length = arrayLength;
                MinItemLength <= length && start + length <= arrayLength;
                length--
            )
            {
                var item = currentCheck.ParserArray[start..(length + start)];
                if (CountBehind(targets, currentIndex, item))
                {
                    return true; // 找到即返回，优雅地跳出所有循环
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 减字查找
    /// </summary>
    /// <summary>
    /// 检索后面的array，统计item所有出现次数
    /// </summary>
    /// <param name="checkTargets"></param>
    /// <param name="repeatItems"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    private bool CountBehind(IList<CheckTarget> checkTargets, int index, TArrayItem[] item)
    {
        // TODO 可以再一个参数最低出现次数，返回值修改为bool（是否达到最低值）
        // TODO 还可以做一个版本，不统计所有次数，出现一定次数后停止然后返回bool
        if (!RepeatItemsList.Any(x => x.Items.SequenceEqual(item)))
        {
            RepeatItem repeatitem = new(item);
            for (var behindIndex = index + 1; behindIndex < checkTargets.Count; behindIndex++)
            {
                var behindTarget = checkTargets[behindIndex];

                //TODO 这里应该用相似匹配
                var count = CountRepeat(behindTarget.ParserArray, item);
                if (count > 0)
                {
                    repeatitem.Count += count;
                    repeatitem.CheckArrays.Add(behindTarget);
                }
            }
            if (repeatitem.Count >= MinOccurTimes)
            {
                RepeatItemsList.Add(repeatitem);
                Console.WriteLine(repeatitem.Count);
                foreach (var item1 in repeatitem.Items)
                {
                   Debug. WriteLine(item1);
                }
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 一个短数组在长数组中重复出现的次数（原位滑动窗口比对）
    /// </summary>
    private static int CountRepeat(TArrayItem[] longArray, TArrayItem[] shortArray)
    {
        var count = 0;
        for (int index = 0; index + shortArray.Length <= longArray.Length; index++)
        {
            var comparor = longArray[index..(index + shortArray.Length)];
            if (shortArray.SequenceEqual(comparor))
            {
                count++;
            }
        }
        return count;
    }
    public class RepeatItem(TArrayItem[] content)
    {
        /// <summary>
        /// 重复数组
        /// </summary>
        public TArrayItem[] Items { set; get; } = content;

        /// <summary>
        /// 重复次数
        /// </summary>
        public int Count { set; get; } = 1;

        /// <summary>
        /// 在哪些checktarget里出现过
        /// </summary>
        public List<CheckTarget> CheckArrays { get; } = [];
    }

    public class CheckTarget(TSource source)
    {
        /// <summary>
        /// 数据源
        /// </summary>
        public TSource Source => source;

        /// <summary>
        /// 要进行比较的数组
        /// </summary>
        public TArrayItem[] ParserArray { get; set; } = null!;
    }
}
