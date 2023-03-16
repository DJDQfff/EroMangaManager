using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.Core.Models;
using EroMangaManager.Core.ViewModels;
using EroMangaManager.UWP.Helpers;

using Windows.Storage;

namespace EroMangaManager.UWP.Models
{
    internal static class ModelFactory
    {
        /// <summary>ViewModel初始化</summary>
        public static async Task InitialIzeFoldersViewModel(ObservableCollectionVM ViewModel, IEnumerable<StorageFolder> storageFolders)
        {
            ViewModel.MangaFolders.Clear();
            ViewModel.NonZipList.Clear();

            //TODO 现在这个是顺序执行，试试多线程方法，加快速度
            List<Task> tasks = new List<Task>();

            foreach (var folder in storageFolders)
            {
                MangasFolder mangasFolder = new MangasFolder(folder.Path);
                ViewModel.MangaFolders.Add(mangasFolder);


            };
            foreach (var folder in ViewModel.MangaFolders)
            {
                var tempfolder = folder;
                var storage = storageFolders.Single(x => x.Path == folder.FolderPath);
                await ModelFactory.InitialMangasFolder(tempfolder, storage);
                //var task = Task.Run(async () => await ModelFactory.InitialMangasFolder(tempfolder, storage));
                //tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        public static async Task InitialMangasFolder(MangasFolder mangasFolder, StorageFolder StorageFolder)
        {
            mangasFolder.IsInitialing = true;
            var files = await StorageFolder.GetFilesAsync();

            for (int i = 0; i < files.Count; i++)
            {
                StorageFile storageFile = files[i];

                string extension = Path.GetExtension(storageFile.Path).ToLower();

                if (extension != ".zip"/*&& extension!=".rar"*/)
                {
                    break;  // 如果不是zip文件，则跳过
                }

                MangaBook manga = await ModelFactory.CreateMangaBook(storageFile);

                // TODO 使用多线程的话总是在这出问题
                mangasFolder.MangaBooks.Add(manga);

            }
            mangasFolder.IsInitialing = false;

        }
        public static async Task<MangaBook> CreateMangaBook(StorageFile storageFile)
        {
            var filepath = storageFile.Path;
            MangaBook mangaBook = new MangaBook(filepath);
            mangaBook.FileSize = (await storageFile.GetBasicPropertiesAsync()).Size;
            var coverpath = await Helpers.CoverHelper.TryCreatCoverFileAsync(storageFile);
            mangaBook.CoverPath = coverpath ?? CoverHelper.DefaultCoverPath;

            return mangaBook;

        }
    }
}
