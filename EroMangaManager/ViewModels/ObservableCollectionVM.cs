using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.Models;

using EroMangaDB.Entities;
using EroMangaDB.EntityFactory;

using MyUWPLibrary;

using Windows.Storage;

using static EroMangaDB.BasicController;
using static Windows.Storage.AccessCache.StorageApplicationPermissions;

namespace EroMangaManager.ViewModels
{
    /// <summary>
    /// 所有需要持续观察的集合都放在这，ViewModel
    /// </summary>
    public class ObservableCollectionVM
    {
        /// <summary>
        /// 出现无法解析的Manga时引发
        /// </summary>
        public event Action<string> ErrorZipEvent;
        internal ObservableCollection<MangasFolder> MangaFolders {  get; } = new ObservableCollection<MangasFolder>();
        /// <summary>存放zip文件的文件夹</summary>
        internal ObservableCollection<StorageFolder> FolderList {  get; } = new ObservableCollection<StorageFolder>();

        /// <summary>各漫画zip</summary>
        internal ObservableCollection<MangaBook> MangaList {  get; } = new ObservableCollection<MangaBook>();

        /// <summary>流的内容不是 zip 存档格式。</summary>
        internal ObservableCollection<MangaBook> NonZipList { get; } = new ObservableCollection<MangaBook>();

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="storageFolders">添加的文件夹</param>
        public ObservableCollectionVM (params StorageFolder[] storageFolders)
        {
            Initialize(storageFolders);
        }

        public void ErrorMangaEvent (string manganame)
        {
            ErrorZipEvent?.Invoke(manganame);
        }
        /// <summary>ViewModel初始化</summary>
        public async void Initialize (params StorageFolder[] storageFolders)
        {
            FolderList.Clear();
            MangaList.Clear();//初始化清零
            NonZipList.Clear();

            foreach (var item in storageFolders)
            {
                if (item != null)
                {
                    FolderList.Add(item);
                }
            }

            List<Task> tasks = new List<Task>();    
            foreach (var folder in FolderList)
            {
                MangasFolder mangasFolder = new MangasFolder(folder);
                MangaFolders.Add(mangasFolder);
              Task task=new Task(async ()=>await mangasFolder.Initial());
                 tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 添加文件夹，并添加文件夹及下属漫画漫画到集合
        /// 1.添加改文件夹到系统API
        /// 2.添加文件夹到FolderList
        /// 3.添加文件夹下属漫画到MangaList
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public async Task AddFolder (StorageFolder folder)
        {
            FutureAccessList.Add(folder);

            if (! FolderList.Contain(folder))
            {
                FolderList.Add(folder);

                MangasFolder mangasFolder = new MangasFolder(folder);
                MangaFolders.Add(mangasFolder);
                await mangasFolder.Initial();
            }
        }

        /// <summary>
        /// 移除文件夹，并从集合中移除文件夹及下属漫画 （只移除，不删除）
        /// 1.从系统API中移除 
        /// 2.从FolderList里移除 
        /// 3.从MangaList里移除文件夹下属漫画
        /// </summary>
        /// <param name="folder"></param>
        internal void RemoveFolder (MangasFolder mangasfolder)
        {
            var folder = mangasfolder.StorageFolder;
            FolderList.Remove(folder);      // 从访问列表Model里移除

            string token = FutureAccessList.Add(folder);        // 获取系统存储token

            FutureAccessList.Remove(token); // 从系统未来访问列表里删除

            MangaFolders.Remove(mangasfolder);
        }

        #region 弃用
        [Obsolete]
        /// <summary>从MangaList中移除特定文件夹下的漫画</summary>
        /// <param name="folderpath"></param>
        private void RemoveMangaInFolder (string folderpath)
        {
            for (int i = MangaList.Count - 1 ; i >= 0 ; i--)
            {
                var manga = MangaList[i];
                if (manga.FolderPath == folderpath)
                {
                    MangaList.Remove(manga);
                }
            }
        }

        /// <summary>读取数据库，将指定文件夹下的 MangaTag 移除</summary>
        /// <param name="storageFolder"></param>
        /// <returns></returns>
        [Obsolete]
        private async Task RemoveMultiTagsFromDatabase (StorageFolder storageFolder)
        {
            var files = (await storageFolder.GetFilesAsync()).Select(n => n.Path).ToArray();
            await DatabaseController.MangaTag_RemoveMulti(files);
        }

        /// <summary>从数据库中移除指定漫画的 MangaTag</summary>
        /// <param name="mangaBook"></param>
        /// <returns></returns>
        public async Task DeleteSingleMangaBook (MangaBook mangaBook)
        {
            MangaList.Remove(mangaBook);
            await DatabaseController.MangaTag_RemoveSingle(mangaBook.StorageFile.Path);

            await mangaBook.StorageFile.DeleteAsync();
        }

        #endregion 弃用
    }
}