using CommonLibrary.CollectionFindRepeat;
using CommonLibrary.RepetitiveGroup;
using static CommonLibrary.BracketBasedStringParser;

namespace Core.ViewModels;

/// <summary>
/// 查找重复manga的viewmodel
/// </summary>
[ObservableObject]
public partial class SameMangaSearchViewModel
    : RepeatItemsGroupWithMethod<string, Manga, RepeatMangasGroup>
{
    /// <summary>
    /// 表示查重方法执行中
    /// </summary>
    private readonly char[] sperators =
    [
        ' ',
        '&',
        '+',
        '～',
        '~',
        '#',
        '!',
        '?',
        '？',
        '！',
        '|',
        '丶',
        '、',
        '•',
        '﹐',
    ];

    [ObservableProperty]
    public partial bool IsWorking { get; set; } = false;

    private static bool FiltKeystring(string? str) => string.IsNullOrWhiteSpace(str);

    /// <summary>
    ///
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task Method3_2(
        IEnumerable<string> tags,
        CancellationTokenSource cancellationTokenSource
    )
    {
        RepeatPairs.Clear();
        foreach (var tag in tags)
        {
            var mangas = Source.Where(x => x.Tags.Contains(tag)).OrderBy(x => x.Tags.Length);
            Source = [.. Source.Except(mangas)];

            string? func1(Manga manga1, Manga manga2)
            {
                //var tags1 = manga1.Tags;
                //var tags2 = manga2.Tags;

                var namepieces1 = manga1.Name.Split(sperators);
                var namepieces2 = manga2.Name.Split(sperators);
                var intersect = namepieces1.Intersect(namepieces2); //.Any();
                if (intersect.Any())
                {
                    return /*$"[{tag}]" + "\t" +*/
                    intersect.First();
                }
                return null;
            }

            await StartCompareSequence([.. mangas], func1, FiltKeystring, cancellationTokenSource);
        }
    }

    /// <summary>
    /// 先传入tag集合，对每个tag，找出重复的本子
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task Method3_1(
        IEnumerable<string> tags,
        CancellationTokenSource cancellationTokenSource
    )
    {
        RepeatPairs.Clear();
        foreach (var tag in tags)
        {
            var mangas = Source.Where(x => x.Tags.Contains(tag)).ToList();
            Source = [.. Source.Except(mangas)];
            if (mangas.Count < 2)
            {
                continue;
            }

            IEnumerable<string> func(IEnumerable<Manga> _mangas)
            {
                StringCollection<Manga, string> stringCollection = new()
                {
                    Action = x =>
                        x.Name /*.ToCharArray();//*/
                        .Split(sperators),
                    Sources = mangas,
                    MinItemLength = 1,
                };
                stringCollection.Run2();
                var keys = stringCollection.RepeatItemsList.Select(x => string.Join(' ', x.Items));
                return keys;
            }

            // TODO 每个key还需要检查一边包含关系
            await ParseAll_FindOut(mangas, func, (x, key) => x.Name.Contains(key), FiltKeystring);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    [Obsolete("方法有bug")]
    public async Task Method3_0(
        IEnumerable<string> tags,
        CancellationTokenSource cancellationTokenSource
    )
    {
        RepeatPairs.Clear();
        foreach (var tag in tags)
        {
            var mangas = Source.Where(x => x.Tags.Contains(tag)).ToList();
            string? func(Manga x, Manga y)
            {
                if (x.Tags.Contains(tag) && y.Tags.Contains(tag))
                {
                    var piecesx = Get_OutsideContent(x.Name);
                    var piecesy = Get_OutsideContent(y.Name);
                    if (piecesx.Intersect(piecesy).Any())
                        return "[" + tag + "]" + piecesx.Intersect(piecesy).First();
                }
                return null;
            }
            var _viewmodel = new SameMangaSearchViewModel();
            await _viewmodel.StartCompareSequence(
                mangas,
                func,
                FiltKeystring,
                cancellationTokenSource
            );

            foreach (var pair in _viewmodel.RepeatPairs)
            {
                RepeatPairs.Add(pair);
            }

            //await StartCompareSequence(mangas , func ,filtKeystring);
        }
    }

    //
    /// <summary>
    /// 先找出第一次重复的tag和manganame，然后以此为key，循环查找
    /// TODO 需要优化
    /// </summary>
    /// <returns></returns>
    public async Task Method2(CancellationTokenSource cancellationTokenSource)
    {
        RepeatPairs.Clear();
        static string? func1(Manga manga1, Manga manga2)
        {
            var tags1 = manga1.Tags;
            var tags2 = manga2.Tags;

            var namepieces1 = Get_OutsideContent(manga1.FileDisplayName);
            var namepieces2 = Get_OutsideContent(manga2.FileDisplayName);

            if (tags1.Intersect(tags2).Any() && namepieces1.Intersect(namepieces2).Any())
            {
                return tags1.Intersect(tags2).First()
                    + "|"
                    + namepieces1.Intersect(namepieces2).First();
            }
            return null;
        }

        await StartCompareSequence(Source, func1, FiltKeystring, cancellationTokenSource);
    }

    /// <summary>
    /// 将所有manganame分别切成小段，将所有小段重组成一个字典，各小段设为key，所有含有这个key的视为重复组
    /// </summary>
    /// <returns></returns>
    public async Task Method1()
    {
        RepeatPairs.Clear();
        Func<Manga, string> func = default!;

        var dic = StringArrayCollection
            .Run(
                Source,
                x => Get_OutsideContent(x.FileDisplayName).SelectMany(x => x.Split(sperators))
            )
            .Where(x => x.Value > 1)
            .Where(x => !int.TryParse(x.Key, out _))
            .Where(x => !char.TryParse(x.Key, out _))
            .ToDictionary();
        func = x =>
            Get_OutsideContent(x.FileDisplayName)
                .SelectMany(x => x.Split(sperators))
                .First(y => dic.ContainsKey(y));
        await ByEachKey(Source, func, x => !string.IsNullOrWhiteSpace(x.Key));
    }

    /// <summary>
    /// 遍历找出两个相同manganame，以这个manganame为key，所有相同manganame视为重复组
    /// </summary>
    /// <returns></returns>
    public async Task Method0(CancellationTokenSource cancellationTokenSource)
    {
        RepeatPairs.Clear();
        static string? func1(Manga x, Manga y) => x.Name == y.Name ? x.Name : null;
        await StartCompareSequence(Source, func1, FiltKeystring, cancellationTokenSource);
    }
}
