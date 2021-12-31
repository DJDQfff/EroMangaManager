﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

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
        private ObservableCollection<ZipArchiveEntry> zipArchiveEntries { set; get; } = new ObservableCollection<ZipArchiveEntry>();

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

        /// <summary> 对自身的可观察集合添加项 </summary>
        public void OpenEntries (ObservableCollection<ZipArchiveEntry> entries)
        {
            foreach (var entry in zipArchive.Entries)
            {
                bool cansue = entry.EntryFilter();
                if (cansue)
                {
                    entries.Add(entry);
                }
            }
        }

        public async Task OpenImages (ObservableCollection<BitmapImage> bitmapImages)
        {
            foreach (var entry in zipArchiveEntries)
            {
                BitmapImage bitmapImage = await ShowEntryAsync(entry);

                bitmapImages.Add(bitmapImage);
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