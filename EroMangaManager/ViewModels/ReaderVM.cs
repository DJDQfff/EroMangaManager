using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.Helpers;
using EroMangaManager.Models;

using SharpCompress.Archives;

using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.Helpers.ZipEntryHelper;

namespace EroMangaManager.ViewModels
{
    /// <summary>
    /// 阅读页面的ViewModel
    /// </summary>
    public class ReaderVM : IDisposable
    {
        /// <summary> </summary>
        public MangaBook manga { set; get; }

        /// <summary> 打开的文件流 </summary>
        private Stream stream { set; get; }

        /// <summary> 压缩文件 </summary>
        private IArchive zipArchive { set; get; }

        /// <summary> 视图模型可以打开的压缩图片合集 </summary>
        public ObservableCollection<IArchiveEntry> zipArchiveEntries { set; get; } = new ObservableCollection<IArchiveEntry>();

        /// <summary>
        /// 图源
        /// </summary>
        public ObservableCollection<BitmapImage> bitmapImages { set; get; } = new ObservableCollection<BitmapImage>();

        /// <summary> 初始化</summary>
        /// <param name="_manga"> </param>
        public ReaderVM (MangaBook _manga)
        {
            this.manga = _manga;
            stream = manga.StorageFile.OpenStreamForReadAsync().Result;
            zipArchive = ArchiveFactory.Open(stream);
        }

        //TODO 把selectentry和showentry分开
        //TODO 给selectentry添加一个参数，要过滤的数据库
        /// <summary> 从压缩文件的所有entry中，筛选出符合条件的 </summary>
        public async Task SelectEntriesAsync (bool IsFilterImageOn , bool whetherShow = true)
        {
            Stream TempStream = await manga.StorageFile.OpenStreamForReadAsync();

            var TempZipArchive = ArchiveFactory.Open(TempStream);

            var names = TempZipArchive.SortEntriesByName();

            foreach (var name in names)
            {
                var TempEntry = TempZipArchive.Entries.Single(n => n.Key == name);
                bool cansue = await Task.Run(() => TempEntry.EntryFilter(IsFilterImageOn)); // 放在这里可以

                if (cansue)
                {
                    var entry = zipArchive.Entries.Single(n => n.Key == name);
                    zipArchiveEntries.Add(entry);// 异步操作不能放在这里，会占用线程
                    if (whetherShow)
                    {
                        bitmapImages.Add(await ShowEntryAsync(entry));
                    }
                }
            }
        }

        /// <summary> </summary>
        public void Dispose ()
        {
            zipArchiveEntries.Clear();

           
            bitmapImages.Clear();
            zipArchive.Dispose();
            stream.Dispose();
        }
    }
}