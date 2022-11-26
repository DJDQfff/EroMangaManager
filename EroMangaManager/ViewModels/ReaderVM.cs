using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EroMangaManager.Helpers;
using System;
using Windows.UI.Xaml.Media.Imaging;
using EroMangaDB.Entities;
using EroMangaManager.Models;

using static EroMangaManager.Helpers.ZipEntryHelper;

using SharpCompress.Archives;

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

        /// <summary> 构造</summary>
        /// <param name="_manga"> </param>
        private ReaderVM (MangaBook _manga)
        {
            this.manga = _manga;
        }

        private async Task Open ()
        {
            stream = await manga.StorageFile.OpenStreamForReadAsync();
            zipArchive = ArchiveFactory.Open(stream);
        }

        /// <summary> 从压缩文件的所有entry中，筛选出符合条件的 </summary>
        public async Task SelectEntriesAsync ()
        {
            Stream TempStream = await manga.StorageFile.OpenStreamForReadAsync();

            var TempZipArchive = ArchiveFactory.Open(TempStream);

            var names = TempZipArchive.SortEntriesByName();

            foreach (var name in names)
            {
                var TempEntry = TempZipArchive.Entries.Single(n => n.Key == name);
                bool cansue = await Task.Run(() => TempEntry.EntryFilter()); // 放在这里可以

                if (cansue)
                {
                    var entry = zipArchive.Entries.Single(n => n.Key == name);
                    zipArchiveEntries.Add(entry);// 异步操作不能放在这里，会占用线程
                    bitmapImages.Add(await ShowEntryAsync(entry));
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

        /// <summary> 工厂模式创建Reader视图模型 </summary>
        /// <param name="manga"> </param>
        /// <param name="imageFilters"> 要过滤的图片数据库 </param>
        /// <returns> </returns>
        public static async Task<ReaderVM> Creat (MangaBook manga, IEnumerable<ImageFilter> imageFilters)
        {
            ReaderVM reader = new ReaderVM(manga);
            await reader.Open();
            return reader;
        }
    }
}