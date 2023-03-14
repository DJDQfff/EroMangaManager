using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Helper;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Helpers;
using EroMangaManager.UWP.Views.MainPageChildPages;

using Windows.Storage;
using EroMangaManager.Core.ViewModels;
namespace EroMangaManager.UWP.Models
{
    internal static class ModelFactory
    {         
        /// <summary>ViewModel初始化</summary>
       public static async Task InitialIzeFoldersViewModel (ObservableCollectionVM ViewModel, IEnumerable<StorageFolder> storageFolders)
        {
                ViewModel. MangaFolders.Clear();
            ViewModel.NonZipList.Clear();

                //TODO 现在这个是顺序执行，试试多线程方法，加快速度
                foreach (var folder in storageFolders)
                {
                    MangasFolder mangasFolder = new MangasFolder(folder.Path);
                ViewModel.MangaFolders.Add(mangasFolder);
                };

                foreach (var folder in ViewModel.MangaFolders)
                {
                    var storage = App.Current.storageItemManager.GetStorageFolder(folder.FolderPath);
                    await ModelFactory.InitialMangasFolder(folder , storage);
                }

        }

        public static async Task InitialMangasFolder (MangasFolder mangasFolder , StorageFolder StorageFolder)
        {
            var files = await StorageFolder.GetFilesAsync();

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

                MangaBook manga = await ModelFactory.CreateMangaBook(storageFile );

                try
                {
                   mangasFolder. MangaBooks.Add(manga);
                }
                catch (Exception)
                {
                    mangasFolder.ErrorBooks.Add(manga);
                    App.Current.GlobalViewModel.ErrorMangaEvent(manga.MangaName);
                }
            }

        }
        public static async Task<MangaBook> CreateMangaBook (StorageFile storageFile )
        { 
             var filepath = storageFile.Path;
            MangaBook mangaBook = new MangaBook ( filepath);
            mangaBook.FileSize = (await storageFile.GetBasicPropertiesAsync()).Size;
            var coverpath = await Helpers.CoverHelper.TryCreatCoverFileAsync(storageFile);
            mangaBook.CoverPath = coverpath ?? CoverHelper.DefaultCoverPath;

            return mangaBook;

        }
    }
}
