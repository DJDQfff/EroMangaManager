using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB;

using EroMangaManager.Core.Models;
using EroMangaManager.Core.ViewModels;
using EroMangaManager.UWP.Helpers;

using MyLibrary.Standard20;

using Windows.Storage;

namespace EroMangaManager.UWP.Models
{
    /// <summary>
    /// 基于该平台的实例创建方法
    /// </summary>
    internal static class ModelFactory
    {
        /// <summary>ViewModel初始化</summary>
        public static async Task InitialIzeFoldersViewModel(ObservableCollectionVM ViewModel, IEnumerable<StorageFolder> storageFolders)
        {
            ViewModel.MangaFolders.Clear();

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
            }
        }

        /// <summary>
        /// 添加MangaBook，并在后台初始化封面
        /// </summary>
        /// <param name="mangasFolder"></param>
        /// <param name="StorageFolder"></param>
        /// <returns></returns>
        public static async Task InitialMangasFolder(MangasFolder mangasFolder, StorageFolder StorageFolder)
        {
            mangasFolder.IsInitialing = true;
            var files = await StorageFolder.GetFilesAsync();
            var filteredfiles = files.Where(x => Path.GetExtension(x.Path).ToLower() == ".zip").ToList();

            var a = BasicController.DatabaseController.database.FilteredImages.ToArray();
            List<Task> tasks = new List<Task>();

            LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(2);
            TaskFactory taskFactory = new TaskFactory(lcts);
            foreach (var storageFile in filteredfiles)
            {
                var file = storageFile;

                MangaBook manga = await ModelFactory.CreateMangaBook(file);

                mangasFolder.MangaBooks.Add(manga);

                Task task = taskFactory.StartNew(async () => manga.CoverPath = (await Helpers.CoverHelper.TryCreatCoverFileAsync(file, a)) ?? CoverHelper.DefaultCoverPath);
                tasks.Add(task);
            }
            mangasFolder.IsInitialing = false;

            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 初始化MangaBook，同时初始化FileSize和CoverPath
        /// </summary>
        /// <param name="storageFile"></param>
        /// <returns></returns>
        public static async Task<MangaBook> CreateMangaBook(StorageFile storageFile)
        {
            var filepath = storageFile.Path;
            MangaBook mangaBook = new MangaBook(filepath)
            {
                FileSize = (await storageFile.GetBasicPropertiesAsync()).Size,

                CoverPath = CoverHelper.DefaultCoverPath
            };

            return mangaBook;
        }
    }
}