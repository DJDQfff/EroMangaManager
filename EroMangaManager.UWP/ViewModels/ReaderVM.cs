using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB.Entities;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Helpers;

using SharpCompress.Archives;
using SharpCompress.Archives.Zip;

using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

using static EroMangaManager.UWP.Helpers.ZipEntryHelper;

namespace EroMangaManager.UWP.ViewModels
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
        public MangaBook Manga { set; get; }

        public StorageFile StorageFile { set; get; }

        /// <summary> 打开的文件流 </summary>
        private Stream Stream { set; get; }

        /// <summary> 压缩文件 </summary>
        private IArchive ZipArchive { set; get; }

        /// <summary>
        /// 压缩文件的所有entry
        /// </summary>
        public List<IArchiveEntry> AllEntries { set; get; }

        private IArchiveEntry currentEntry;

        private bool isworking;

        private Queue<IArchiveEntry> entries = new Queue<IArchiveEntry>();

        /// <summary>筛选过后的图片内容入口 </summary>
        public ObservableCollection<IArchiveEntry> FilteredArchiveImageEntries { set; get; } = new ObservableCollection<IArchiveEntry>();

        /// <summary>
        /// 对应词典
        /// </summary>
        public Dictionary<IArchiveEntry, BitmapImage> BitmapImagesDic { set; get; } = new Dictionary<IArchiveEntry, BitmapImage>();

        /// <summary>
        /// 图源
        /// </summary>
        public ObservableCollection<BitmapImage> BitmapImages { set; get; } = new ObservableCollection<BitmapImage>();

        /// <summary>
        ///
        /// </summary>
        /// <param entrykey="_manga"></param>
        /// <param entrykey="storageFile"></param>
        public ReaderVM(MangaBook _manga, StorageFile storageFile)
        {
            this.Manga = _manga;

            StorageFile = storageFile;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public async Task Initial()
        {
            Stream = await StorageFile.OpenStreamForReadAsync();
            ZipArchive = ArchiveFactory.Open(Stream);
            AllEntries = ZipArchive.Entries.ToList();
        }

        /// <summary>
        /// 显示指定entry，并添加到图片字典中
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<BitmapImage> ShowSpecificBitmapImage(IArchiveEntry entry)
        {
            if (isworking)
            {
                return null;
            }

            isworking = true;
            BitmapImage bitimage;
            if (!BitmapImagesDic.ContainsKey(entry))
            {
                bitimage = await entry.ShowEntryAsync();

                BitmapImages.Add(bitimage);
                BitmapImagesDic[entry] = bitimage;
            }
            else
            {
                bitimage = BitmapImagesDic[entry];
            }
            isworking = false;
            return bitimage;
        }

        /// <summary>
        /// 所有筛选过的entry一次性转化为图片
        /// </summary>
        /// <returns></returns>
        public async Task ShowFilteredBitmapImages()
        {
            foreach (var entry in FilteredArchiveImageEntries)
            {
                if (_IsClosing)
                {
                    return;
                }
                //BitmapImage bitmapImage = await entry.ShowEntryAsync();
                //BitmapImages.Add(bitmapImage);
                _ = await ShowSpecificBitmapImage(entry);
            }
        }

        /// <summary> 从压缩文件的所有entry中，筛选出符合条件的，传入null则为不进行筛选 </summary>
        public void SelectEntries(FilteredImage[] filteredImages)
        {
            var entrykeys = ZipArchive.SortEntriesByName();

            foreach (var entrykey in entrykeys)
            {
                var TempEntry = ZipArchive.Entries.Single(n => n.Key == entrykey);
                bool cansue = TempEntry.EntryFilter(filteredImages); // 放在这里可以

                if (cansue)
                {
                    var entry = ZipArchive.Entries.Single(n => n.Key == entrykey);
                    FilteredArchiveImageEntries.Add(entry);// 异步操作不能放在这里，会占用线程
                }
            }
        }

        /// <summary> </summary>
        public void Dispose()
        {
            _IsClosing = true;

            FilteredArchiveImageEntries?.Clear();

            BitmapImages?.Clear();
            ZipArchive?.Dispose();
            Stream?.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}