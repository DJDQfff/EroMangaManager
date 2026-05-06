using CommonLibrary.CollectionFindRepeat;
using CommonLibrary.RepetitiveGroup;
using System.Threading;
using System.Threading.Tasks;
using static CommonLibrary.BracketBasedStringParser;

namespace EroMangaManager.Core.ViewModels;

/// <summary>
/// 查找重复manga的viewmodel
/// </summary>
[ObservableObject]
public partial class SameMangaSearchViewModel : RepeatItemsGroupWithMethod<string, Manga, RepeatMangasGroup>
{
    /// <summary>
    /// 表示查重方法执行中
    /// </summary>
    private char[] sperators = [' ', '&', '+', '～', '~', '#', '!', '?', '？', '！', '|', '丶', '、', '•', '﹐'];

    [ObservableProperty]
    private bool isWorking = false;

    private static bool filtKeystring(string str) => !string.IsNullOrWhiteSpace(str);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task Method3_2(IEnumerable<string> tags, CancellationTokenSource cancellationTokenSource)
    {
        RepeatPairs.Clear();
        foreach (var tag in tags)
        {
            var mangas = Source.Where(x => x.Tags.Contains(tag)).OrderBy(x => x.Tags.Length).ToList();
            Source = Source.Except(mangas).ToList();

            string func1(Manga manga1, Manga manga2)
            {
                //var tags1 = manga1.Tags;
                //var tags2 = manga2.Tags;

                var namepieces1 = manga1.MangaName.Split(sperators);
                var namepieces2 = manga2.MangaName.Split(sperators);
                var intersect = namepieces1.Intersect(namepieces2);//.Any();
                if (intersect.Any())
                {
                    return /*$"[{tag}]" + "\t" +*/ intersect.First();
                }
                return null;
            }

            await StartCompareSequence(mangas, func1, filtKeystring, cancellationTokenSource);
        }
    }

    /// <summary>
    /// 先传入tag集合，对每个tag，找出重复的本子
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    public async Task Method3_1(IEnumerable<string> tags, CancellationTokenSource cancellationTokenSource)
    {
        RepeatPairs.Clear();
        foreach (var tag in tags)
        {
            var mangas = Source.Where(x => x.Tags.Contains(tag)).ToList();
            Source = Source.Except(mangas).ToList();
            if (mangas.Count < 2)
            {
                continue;
            }

            IEnumerable<string> func(IEnumerable<Manga> _mangas)
            {
                StringCollection<Manga, string> stringCollection = new()
                {
                    Action = x => x.MangaName/*.ToCharArray();//*/.Split(sperators),
                    Sources = mangas,
                    MinItemLength = 1
                };
                stringCollection.Run2();
                var keys = stringCollection.RepeatItemsList.Select(x => string.Join(' ', x.Items));
                return keys;
            }

            // TODO 每个key还需要检查一边包含关系
            await ParseAll_FindOut(mangas, func, (x, key) => x.MangaName.Contains(key), filtKeystring);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="tags"></param>
    /// <param name="cancellationTokenSource"></param>
    /// <returns></returns>
    [Obsolete]
    public async Task Method3_0(IEnumerable<string> tags, CancellationTokenSource cancellationTokenSource)
    {
        RepeatPairs.Clear();
        foreach (var tag in tags)
        {
            var mangas = Source.Where(x => x.Tags.Contains(tag)).ToList();
            string func(Manga x, Manga y)
            {
                if (x.Tags.Contains(tag)
                && y.Tags.Contains(tag))
                {
                    var piecesx = Get_OutsideContent(x.MangaName);
                    var piecesy = Get_OutsideContent(y.MangaName);
                    if (piecesx.Intersect(piecesy).Any())
                        return "[" + tag + "]" + piecesx.Intersect(piecesy).First();
                }
                return null;
            }
            var _viewmodel = new SameMangaSearchViewModel();
            await _viewmodel.StartCompareSequence(mangas, func, filtKeystring, cancellationTokenSource);

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
        static string func1(Manga manga1, Manga manga2)
        {
            var tags1 = manga1.Tags;
            var tags2 = manga2.Tags;

            var namepieces1 = Get_OutsideContent(manga1.FileDisplayName);
            var namepieces2 = Get_OutsideContent(manga2.FileDisplayName);

            if (tags1.Intersect(tags2).Any() && namepieces1.Intersect(namepieces2).Any())
            {
                return tags1.Intersect(tags2).First() + "|" + namepieces1.Intersect(namepieces2).First();
            }
            return null;
        }

        await StartCompareSequence(Source, func1, filtKeystring, cancellationTokenSource);
    }

    /// <summary>
    /// 将所有manganame分别切成小段，将所有小段重组成一个字典，各小段设为key，所有含有这个key的视为重复组
    /// </summary>
    /// <returns></returns>
    public async Task Method1()
    {
        RepeatPairs.Clear();
        Func<Manga, string> func = null;

        var dic = StringArrayCollection
    .Run(Source, x => Get_OutsideContent(x.FileDisplayName)
    .SelectMany(x => x.Split(sperators)))

    .Where(x => x.Value > 1)
    .Where(x => !int.TryParse(x.Key, out _))
    .Where(x => !char.TryParse(x.Key, out _))
    .ToDictionary();
        func = x =>
         Get_OutsideContent(x.FileDisplayName)
         .SelectMany(x => x.Split(sperators))
            .FirstOrDefault(y => dic.ContainsKey(y));
        await ByEachKey(Source, func, x => !string.IsNullOrWhiteSpace(x.Key));
    }

    /// <summary>
    /// 遍历找出两个相同manganame，以这个manganame为key，所有相同manganame视为重复组
    /// </summary>
    /// <returns></returns>
    public async Task Method0(CancellationTokenSource cancellationTokenSource)
    {
        RepeatPairs.Clear();
        static string func1(Manga x, Manga y) => x.MangaName == y.MangaName ? x.MangaName : null;
        await StartCompareSequence(Source, func1, filtKeystring, cancellationTokenSource);
    }
}