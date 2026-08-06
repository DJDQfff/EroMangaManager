using System.Threading.Tasks;

namespace Core.Models;

/// <summary>
/// 后台封面加载工作类
/// </summary>
public class CoverSetter
{
    /// <summary>
    ///
    /// </summary>
    public Stack<Queue<Manga>> Stacks { get; } = [];

    private bool changedTop;

    /// <summary>
    /// 后台是否在工作
    /// </summary>
    public bool Isworking { set; get; }

    /// <summary>
    /// 用于设置封面文件路径。因文件创建涉及到winui，且console程序也不显示图片，所以设置封面单独作为委托，需要手动设置
    /// </summary>
    public event Func<Manga, Task> SetCover = null!;

    /// <summary>
    /// 加载漫画基本信息，如大小章节等
    /// </summary>
    public event Func<Manga, Task> MangaInfo = null!;

    /// <summary>
    /// 独立开始加载任务，与队列无关
    /// </summary>
    /// <param name="manga"></param>
    /// <param name="setcover">是否读取封面</param>
    /// <param name="loadinfo">是否获取基本信息</param>
    /// <returns></returns>
    public async Task SingleLoadWork(Manga manga, bool setcover, bool loadinfo)
    {
        if (setcover)
        {
            await SetCover.Invoke(manga);
        }
        if (loadinfo) // 以filesize是否为0，来判断漫画信息是否已初始化
        {
            await MangaInfo.Invoke(manga);
        }
    }

    /// <summary>
    /// Manga的enumerable加载
    /// </summary>
    /// <param name="mangas"></param>
    /// <param name="setcover"></param>
    /// <param name="loadinfo"></param>
    /// <returns></returns>
    public async Task MultiLoadWork(IEnumerable<Manga> mangas, bool setcover, bool loadinfo)
    {
        foreach (var manga in mangas)
        {
            await SingleLoadWork(manga, setcover, loadinfo);
        }
    }

    /// <summary>
    /// 在栈中创建大量创建任务
    /// </summary>
    /// <param name="mangas"></param>
    /// <param name="setcover"></param>
    /// <param name="loadinfo"></param>
    /// <returns></returns>
    [Obsolete(
        "第一个调用addwork后，第二个会直接退出，并不能正确执行原定设计。原定设计第二次触发，则中断第一次的运行任务",
        true
    )]
    public async Task AppendLoadWorks(IEnumerable<Manga> mangas, bool setcover, bool loadinfo)
    {
        Queue<Manga> queue = new(mangas);
        Stacks.Push(queue);
        changedTop = true;

        if (!Isworking)
        {
            Isworking = true;
            while (Stacks.Count > 0)
            {
                var popqueue = Stacks.Peek();

                while (popqueue.Count > 0)
                {
                    if (changedTop)
                    {
                        changedTop = false; // stacks发生改变，处理新queue.
                        break;
                    }

                    var manga = popqueue.Dequeue();

                    await SingleLoadWork(manga, setcover, loadinfo);
                }
                if (popqueue.Count == 0)
                {
                    _ = Stacks.Pop();
                }
            }
            if (Stacks.Count == 0)
            {
                Isworking = false;
            }
        }
    }
}
