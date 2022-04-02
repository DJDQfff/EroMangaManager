using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using EroMangaManager.Helpers;
using System;
using Windows.UI.Xaml.Media.Imaging;
using EroMangaTagDatabase.Entities;
using static EroMangaManager.Helpers.ZipEntryHelper;

namespace EroMangaManager.Models
{
    public class ReaderViewModel : IDisposable
    {
        /// <summary> </summary>
        public MangaBook manga { set; get; }

        /// <summary> 打开的文件流 </summary>
        private Stream stream { set; get; }

        /// <summary> 压缩文件 </summary>
        private ZipArchive zipArchive { set; get; }

        /// <summary> 视图模型可以打开的压缩图片合集 </summary>
        public ObservableCollection<ZipArchiveEntry> zipArchiveEntries { set; get; } = new ObservableCollection<ZipArchiveEntry>();

        /// <summary> </summary>
        /// <param name="_manga"> </param>
        /// <param name="imageFilters"> </param>
        private ReaderViewModel (MangaBook _manga)
        {
            this.manga = _manga;
        }

        private async Task Open ()
        {
            stream = await manga.StorageFile.OpenStreamForReadAsync();
            zipArchive = new ZipArchive(stream);
        }

        /// <summary> 从压缩文件的所有entry中，筛选出符合条件的 </summary>
        public async Task SelectEntriesAsync ()
        {
            Stream TempStream = await manga.StorageFile.OpenStreamForReadAsync();

            ZipArchive TempZipArchive = new ZipArchive(TempStream);

            //TODO 加一个排序
            List<ZipArchiveEntry> sortedEntries = new List<ZipArchiveEntry>();

            for (int i = 0; i < TempZipArchive.Entries.Count; i++)
            {
                var TempEntry = TempZipArchive.Entries[i];
                bool cansue = await Task.Run(() => TempEntry.EntryFilter()); // 放在这里可以

                if (cansue)
                {
                    var entry = zipArchive.Entries[i];
                    zipArchiveEntries.Add(entry);// 异步操作不能放在这里，会占用线程
                }
            }
        }

        /// <summary> </summary>
        public void Dispose ()
        {
            zipArchiveEntries.Clear();
            zipArchive.Dispose();
            stream.Dispose();
        }

        /// <summary> 工厂模式创建Reader视图模型 </summary>
        /// <param name="manga"> </param>
        /// <param name="imageFilters"> 要过滤的图片数据库 </param>
        /// <returns> </returns>
        public static async Task<ReaderViewModel> Creat (MangaBook manga, IEnumerable<ImageFilter> imageFilters)
        {
            ReaderViewModel reader = new ReaderViewModel(manga);
            await reader.Open();
            return reader;
        }
    }
}