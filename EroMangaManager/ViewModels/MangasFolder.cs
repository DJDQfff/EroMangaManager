using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB.Entities;

using EroMangaManager.UWP.Helpers;
using EroMangaManager.UWP.Models;

using Windows.Storage;

namespace EroMangaManager.UWP.ViewModels
{
    internal class MangasFolder : INotifyPropertyChanged

    {
        public StorageFolder StorageFolder { get; }

        private bool _IsInitialiing = false;

        /// <summary>
        /// 是否在更新数据
        /// </summary>
        public bool IsInitialing
        {
            set
            {
                _IsInitialiing = value;
                PropertyChanged?.Invoke(this , new PropertyChangedEventArgs(""));
            }
            get => _IsInitialiing;
        }

        public string FolderPath => StorageFolder.Path;

        public ObservableCollection<MangaBook> MangaBooks { get; } = new ObservableCollection<MangaBook>();
        public ObservableCollection<MangaBook> ErrorBooks { get; } = new ObservableCollection<MangaBook>();

        public MangasFolder (StorageFolder storageFolder)
        {
            StorageFolder = storageFolder;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task Initial ()
        {
            IsInitialing = true;
            var files = await StorageFolder.GetFilesAsync();

            List<ReadingInfo> add = new List<ReadingInfo>();
            // 这里如果使用 list<task> 的话，会出bug
            // 普通的遍历添加反而不会出现bug
            // bug 名称：已为另一线程调用
            for (int i = 0 ; i < files.Count ; i++)
            {
                StorageFile storageFile = files[i];

                string extension = Path.GetExtension(storageFile.Path).ToLower();

                if (extension != ".zip"/*&& extension!=".rar"*/)
                {
                    break;  // 如果不是zip文件，则跳过
                }

                MangaBook manga = new MangaBook(storageFile , StorageFolder);
                manga.CoverPath = CoverHelper.DefaultCoverPath;

                try
                {
                    var path = await Helpers.CoverHelper.TryCreatCoverFileAsync(storageFile);
                    manga.CoverPath = path ?? CoverHelper.DefaultCoverPath;
                    MangaBooks.Add(manga);
                }
                catch (Exception)
                {
                    manga.CoverPath = CoverHelper.DefaultCoverPath;
                    ErrorBooks.Add(manga);
                    App.Current.GlobalViewModel.ErrorMangaEvent(manga.MangaName);
                }
            }

            IsInitialing = false;
        }

        /// <summary>
        /// 对内部漫画进行排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="func"></param>
        public void SortMangaBooks<TKey> (Func<MangaBook , TKey> func)
        {
            var list = MangaBooks.OrderBy(func);        //OrderBy方法不会修改源数据，返回的值是与源挂钩的，源清零，返回值也清零

            var list2 = new List<MangaBook>(list);
            MangaBooks.Clear();

            foreach (var book in list2)
            {
                MangaBooks.Add(book);
            }
        }

        public void RemoveManga (MangaBook mangaBook)
        {
            MangaBooks.Remove(mangaBook);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="mangaBook"></param>
        /// <param name="newdisplayname">不带后缀名的新名字，后缀会自行补上，限定为zip</param>
        /// <returns></returns>
        public async Task RenameManga (MangaBook mangaBook , string newdisplayname)
        {
            await mangaBook.StorageFile.RenameAsync(newdisplayname + ".zip");
        }
    }
}