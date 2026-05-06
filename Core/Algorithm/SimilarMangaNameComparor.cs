namespace EroMangaManager.Core.Algorithm
{
    /// <summary>
    /// 本子名比较器，用于从一堆类似的本子中选出可能相同的本子。
    /// </summary>
    /// <remarks>
    /// 传入要比较的本子
    /// </remarks>
    /// <param name="_books"></param>
    public class SimilarMangaNameComparor(IEnumerable<Manga> _books)
    {
        private readonly List<Manga> books = new(_books);

        private readonly Dictionary<Manga, string> mangaBookDictionary = [];

        /// <summary>
        /// 依据本子名进行比较，找出这个本子的名。
        /// 本子名（如xxx1，xxx2，yyy前篇，yyy后篇，zzz上，zzz下。
        /// 有的本子名既有原名也有译名，有的本子名是译名而不是原名）
        /// 很多喜欢用空格进行分隔
        /// </summary>
        public void CompareByMangaName()
        {
            // dic1里的每个本子，本子名唯一
            // dic2里的每个本子，本子名重合，但是本子不同部分没用括号分开，如：xx1，xx2
            // dic3里的每个本子，本子名重合，但是本子差别部分使用空格分开，如：yy 上，yy 下
            var dic1 = new Dictionary<Manga, string>();
            var dic2 = new Dictionary<Manga, string>();
            var dic3 = new Dictionary<Manga, string[]>();

            var list1 = new List<Manga>();
            var list2 = new List<Manga>();
            //简单分类
            foreach (var book in books)
            {
                if (book.MangaName.Contains(' '))
                {
                    list1.Add(book);
                }
                else
                {
                    list2.Add(book);
                }
            }

            // 对dic2里的本子，找出唯一本子名，挪到dic1里去
            list1.Sort((x, y) => x.MangaName.Length - y.MangaName.Length);
            list1.ForEach(x => Console.WriteLine(x.MangaName));

            // 对dic3里的本子，找出唯一，挪到dic1
        }

        /// <summary>
        /// </summary>
        ///
        /// <returns></returns>
        public string FindUniqueName(Manga book)
        {
            // TODO 看看能不能用迭代器
            if (!books.Contains(book))
            {
                throw new ArgumentException("传入本子不是要比较的一部分");
            }
            else
            {
                return mangaBookDictionary[book];
            }
        }
    }
}