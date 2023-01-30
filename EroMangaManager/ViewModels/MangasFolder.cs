using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB.Entities;
using EroMangaDB.EntityFactory;

using EroMangaManager.Models;

using Windows.Storage;

using static EroMangaDB.BasicController;

namespace EroMangaManager.ViewModels
{
    internal class MangasFolder

    {
        public StorageFolder StorageFolder { get; }
        public string FolderPath => StorageFolder.Path;

        public ObservableCollection<MangaBook> MangaBooks { get; } = new ObservableCollection<MangaBook>();
        public ObservableCollection<MangaBook> ErrorBooks { get; } = new ObservableCollection<MangaBook>();

        public MangasFolder (StorageFolder storageFolder)
        {

            StorageFolder = storageFolder;
        }

        /// <summary>
        /// TODO 把readinginfo分离出来
        /// </summary>
        /// <returns></returns>
        public async Task Initial ()
        {
            var files = await StorageFolder.GetFilesAsync();
            ReadingInfo[] tags = DatabaseController.ReadingInfo_QueryAll();

            List<ReadingInfo> add = new List<ReadingInfo>();
            // 这里如果使用 list<task> 的话，会出bug
            // 普通的遍历添加反而不会出现bug
            // bug 名称：已为另一线程调用
            for (int i = 0 ; i < files.Count ; i++)
            {
                StorageFile storageFile = files[i];

                string extension = Path.GetExtension(storageFile.Path).ToLower();

                if (extension != ".zip")
                {
                    break;  // 如果不是zip文件，则跳过
                }

                ReadingInfo readingInfo;
                try
                {   //
                    readingInfo = tags.Single(n => n.AbsolutePath == storageFile.Path);
                }
                catch (InvalidOperationException)
                {
                    readingInfo = ReadingInfoFactory.Creat(storageFile.Path);
                    add.Add(readingInfo);
                }

                MangaBook manga = new MangaBook(storageFile , StorageFolder , readingInfo);

                try
                {
                    MangaBooks.Add(manga);

                    await Helpers.CoverHelper.TryCreatCoverFileAsync(storageFile);

                    await manga.ChangeCover();
                }
                catch (Exception)
                {
                    MangaBooks.Remove(manga);
                    ErrorBooks.Add(manga);
                    App.Current.collectionObserver.ErrorMangaEvent(manga.MangaName);
                }
            }

            await DatabaseController.ReadingInfo_AddMulti(add);
        }
        /// <summary>
        /// 对内部漫画进行排序
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="func"></param>
        public void SortMangaBooks<TKey>(Func<MangaBook ,TKey> func) 
        {

          var list=MangaBooks.OrderBy(func);        //OrderBy方法不会修改源数据，返回的值是与源挂钩的，源清零，返回值也清零

            var list2 = new List<MangaBook>(list);
            MangaBooks.Clear();

          foreach(var book in list2)
            {
                MangaBooks.Add(book);
            }

        }
        public void RemoveManga (MangaBook mangaBook)
        {
            MangaBooks.Remove(mangaBook);
        }
    }
}