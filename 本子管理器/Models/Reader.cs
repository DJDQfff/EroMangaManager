using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;
using EroMangaManager.Helpers;

using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.Helpers.ZipEntryHelper;

namespace EroMangaManager.Models
{
    public class Reader
    {
        private MangaBook manga { set; get; }
        private Stream stream { set; get; }
        private ZipArchive zipArchive { set; get; }

        private Reader (MangaBook _manga)
        {
            this.manga = _manga;
        }

        public static async Task<Reader> Create (MangaBook manga)
        {
            Reader reader = new Reader(manga);
            await reader.OpenStream();
            reader.OpenArchive();
            Debug.WriteLine(reader.GetHashCode());
            return reader;
        }

        private async Task OpenStream ()
        {
            stream = await manga.StorageFile.OpenStreamForReadAsync();
        }

        private void OpenArchive ()
        {
            zipArchive = new ZipArchive(stream);
        }

        /// <summary> 从压缩文件的所有entry中，筛选出符合条件的 </summary>
        public async Task SelectEntriesAsync (ObservableCollection<ZipArchiveEntry> entries)
        {
            for (int i = 0; i < zipArchive.Entries.Count; i++)
            {
                var entry = zipArchive.Entries[i];
                bool cansue = await Task.Run(() => entry.EntryFilter()); // 放在这里可以

                if (cansue)
                {
                    entries.Add(entry);// 异步操作不能放在这里，会占用线程
                }
            }
        }

        /// <summary> 无法正常释放资源，以后再弄 </summary>
        public void Dispose ()
        {
            // TODO 释放文件
            //zipArchiveEntries = null;
            //zipArchive.Dispose();
            //stream.Dispose();
            //Debug.WriteLine("已释放");
        }
    }
}