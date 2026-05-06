using EroMangaManager.Core.IOOperation;
using System.Threading.Tasks;

namespace EroMangaManager.WinUI3.Models
{
    public class CoverSetter
    {
        public Stack<Queue<Manga>> stacks = [];

        private bool chandedTop;

        public bool Isworking;
        /// <summary>
        /// 设置封面文件路径，因为文件创建涉及到winui，且console程序也不显示图片，所以设置封面单独作为委托，需要手动设置
        /// </summary>
        public Action<Manga> SetCover;
        public async Task AddWork(IEnumerable<Manga> mangas)
        {
            Queue<Manga> queue = new(mangas);
            stacks.Push(queue);
            chandedTop = true;

            if (!Isworking)
            {
                Isworking = true;
                while (stacks.Count > 0)
                {
                    var popqueue = stacks.Peek();
                    while (popqueue.Count > 0)
                    {
                        if (chandedTop)
                        {
                            chandedTop = false; // stacks发生改变，处理新queue
                            break;
                        }
                        var manga = popqueue.Dequeue();
                        if (manga.FileSize == 0) // 以filesize是否为0，来判断漫画信息是否已初始化
                        {
                            await MangaFileIO.LoadMangaInfo(manga);
                            SetCover.Invoke(manga);
                        }
                    }
                    if (popqueue.Count == 0)
                    {
                        _ = stacks.Pop();
                    }
                }
                if (stacks.Count == 0) { Isworking = false; }
            }
        }
    }
}