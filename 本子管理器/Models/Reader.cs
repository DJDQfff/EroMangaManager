﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Helpers;
using EroMangaManager.Models;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.Helpers.ZipEntryHelper;

namespace EroMangaManager.Models
{
    public class Reader
    {
        private StorageFile File { set; get; }
        private Stream stream { set; get; }
        private ZipArchive zipArchive { set; get; }
        private ObservableCollection<ZipArchiveEntry> zipArchiveEntries { set; get; } = new ObservableCollection<ZipArchiveEntry>();
        private ObservableCollection<BitmapImage> bitmapImages { set; get; } = new ObservableCollection<BitmapImage>();

        private Reader (StorageFile file)
        {
            this.File = file;
        }

        public static async Task<Reader> Create (StorageFile file)
        {
            Reader reader = new Reader(file);
            await reader.OpenStream();
            reader.OpenArchive();
            Debug.WriteLine(reader.GetHashCode());
            return reader;
        }

        private async Task OpenStream ()
        {
            stream = await File.OpenStreamForReadAsync();
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