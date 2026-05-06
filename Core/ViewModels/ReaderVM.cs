using EroMangaManager.Core.IOOperation;

namespace EroMangaManager.Core.ViewModels
{
    /// <summary>
    /// 阅读页面的ViewModel
    /// </summary>
    public class ReaderVM : IDisposable
    {
        /// <summary>
        /// 现在是否在打开这个本子，如果是的话，这个为真，否则，为假。设置这个是因为加载所有图片很长，有时候图正在加载中，本子就关闭了，需要这个作为
        /// </summary>
        private bool _IsClosing = false;

        /// <summary> </summary>
        public Manga Manga { set; get; }

        /// <summary> 打开的文件流 </summary>
        private Stream Stream { set; get; }

        /// <summary> 压缩文件 </summary>
        private IArchive ZipArchive { set; get; }

        /// <summary>
        /// 压缩文件的所有entry
        /// </summary>
        public List<IArchiveEntry> AllEntries { set; get; }

        //private IArchiveEntry currentEntry;

        private bool isworking;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:删除未读的私有成员", Justification = "<挂起>")]
        private readonly Queue<IArchiveEntry> entries = new();

        /// <summary>筛选过后的图片内容入口 </summary>
        public ObservableCollection<IArchiveEntry> FilteredArchiveImageEntries { set; get; } = [];

        /// <summary>
        /// 对应词典
        /// </summary>
        public Dictionary<IArchiveEntry, Stream> BitmapImagesDic { set; get; } = [];

        /// <summary>
        ///
        /// </summary>
        /// <param entrykey="_manga"></param>
        /// <param entrykey="storageFile"></param>
        public ReaderVM(Manga _manga)
        {
            Manga = _manga;

            Stream = new FileStream(Manga.FilePath, FileMode.Open, FileAccess.Read);
            ZipArchive = ArchiveFactory.Open(Stream);
            AllEntries = ZipArchive.Entries.ToList();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="stream"></param>
        public ReaderVM(Stream stream)
        {
            Stream = stream;
            ZipArchive = ArchiveFactory.Open(Stream);
            AllEntries = ZipArchive.Entries.ToList();
        }

        /// <summary>
        /// 显示指定entry，并添加到图片字典中, 如果返回null，则未能正常打开流
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Stream GetOrAddShowedEntry(IArchiveEntry entry)
        {
            if (isworking)
            {
                return null;
            }

            try
            {
                Stream bitimage;

                isworking = true;
                if (!BitmapImagesDic.ContainsKey(entry))
                {
                    bitimage = entry.OpenEntryStream();
                    BitmapImagesDic[entry] = bitimage;
                }
                else
                {
                    bitimage = BitmapImagesDic[entry];
                }
                isworking = false;
                return bitimage;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 所有筛选过的entry一次性转化为图片
        /// </summary>
        /// <returns></returns>
        [Obsolete("一次加载所有图片，非常消耗性能，而且在加载完成前对集合进行操作容易出bug")]
        public void ShowFilteredBitmapImages()
        {
            foreach (IArchiveEntry entry in FilteredArchiveImageEntries)
            {
                if (_IsClosing)
                {
                    return;
                }
                //Stream bitmapImage = entry.OpenEntryStream();
                _ = GetOrAddShowedEntry(entry);
            }
        }

        /// <summary> 从压缩文件的所有entry中，筛选出符合条件的，传入null则为不进行筛选 </summary>
        public void SelectEntries(FilteredImage[] filteredImages)
        {
            List<string> entrykeys = ZipArchive.SortEntriesByName();

            foreach (string entrykey in entrykeys)
            {
                IArchiveEntry TempEntry = ZipArchive.Entries.Single(n => n.Key == entrykey);
                bool cansue = TempEntry.EntryFilter(filteredImages); // 放在这里可以

                if (cansue)
                {
                    IArchiveEntry entry = ZipArchive.Entries.Single(n => n.Key == entrykey);
                    FilteredArchiveImageEntries.Add(entry);// 异步操作不能放在这里，会占用线程
                }
            }
        }

        /// <summary> </summary>
        public void Dispose()
        {
            _IsClosing = true;

            FilteredArchiveImageEntries?.Clear();

            ZipArchive?.Dispose();
            Stream?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}