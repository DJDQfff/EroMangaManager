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

        public static async Task InitialMangasFolder(MangasFolder mangasFolder, StorageFolder StorageFolder)
        {
            mangasFolder.IsInitialing = true;
            var files = await StorageFolder.GetFilesAsync();
            var filteredfiles = files.Where(x => Path.GetExtension(x.Path).ToLower() == ".zip").ToList();

            foreach (var storageFile in filteredfiles)
            {
                MangaBook manga = await ModelFactory.CreateMangaBook(storageFile);

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