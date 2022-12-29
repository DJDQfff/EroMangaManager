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
                    MainPage.current.collectionObserver.ErrorMangaEvent(manga.MangaName);
                }
            }

            await DatabaseController.ReadingInfo_AddMulti(add);
        }

        public void RemoveManga(MangaBook mangaBook)
        {
            MangaBooks.Remove(mangaBook);
        }
    }
}