using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using EroMangaManager.Core.Models;

namespace EroMangaManager.Core.ViewModels
{
    public class MangasFolder : INotifyPropertyChanged
    {
        public string FolderPath { get; }

        private bool _IsInitialiing = false;

        /// <summary>
        /// 是否在更新数据
        /// </summary>
        public bool IsInitialing
        {
            set
            {
                _IsInitialiing = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            }
            get => _IsInitialiing;
        }

        public ObservableCollection<MangaBook> MangaBooks { get; } = new ObservableCollection<MangaBook>();

        public MangasFolder(string storageFolder)
        {
            FolderPath = storageFolder;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 对内部漫画进行排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="func"></param>
        public void SortMangaBooks<TKey>(Func<MangaBook, TKey> func)
        {
            var list = MangaBooks.OrderBy(func);        //OrderBy方法不会修改源数据，返回的值是与源挂钩的，源清零，返回值也清零

            var list2 = new List<MangaBook>(list);
            MangaBooks.Clear();

            foreach (var book in list2)
            {
                MangaBooks.Add(book);
            }
        }

        public void RemoveManga(MangaBook mangaBook)
        {
            MangaBooks.Remove(mangaBook);
        }

    }
}