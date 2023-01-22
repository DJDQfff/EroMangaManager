using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.Models;

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

        /// <summary>
        /// 完成某项任务时引发
        /// </summary>
        public event Action<string> WorkDoneEvent;

        internal ObservableCollection<MangasFolder> MangaFolders { get; } = new ObservableCollection<MangasFolder>();

        /// <summary>存放zip文件的文件夹</summary>
        internal List<StorageFolder> StorageFolders => MangaFolders.Select(n => n.StorageFolder).ToList();

        /// <summary>各漫画zip</summary>
        ///
        internal List<MangaBook> MangaList
        {
            get
            {
                var list = new List<MangaBook>();
                foreach (var folder in MangaFolders)
                {
                    list.AddRange(folder.MangaBooks);
                }
                return list;
            }
        }

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

        /// <summary>ViewModel初始化</summary>
        public async void Initialize (params StorageFolder[] storageFolders)
        {
            MangaFolders.Clear();

            NonZipList.Clear();

            //TODO 现在这个是顺序执行，试试多线程方法，加快速度
            foreach (var folder in storageFolders)
            {
                MangasFolder mangasFolder = new MangasFolder(folder);
                MangaFolders.Add(mangasFolder);
                await mangasFolder.Initial();
            };
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

            if (MangaFolders.FirstOrDefault(x => x.FolderPath == folder.Path) is null)
            {
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
        internal void RemoveFolder (MangasFolder mangasfolder)
        {
            MangaFolders.Remove(mangasfolder);

            string token = FutureAccessList.Add(mangasfolder.StorageFolder);        // 获取系统存储token

            FutureAccessList.Remove(token); // 从系统未来访问列表里删除
        }

        /// <summary>从数据库中移除指定漫画的 MangaTag</summary>
        /// <param name="mangaBook"></param>
        /// <param name="deleteOption">删除模式</param>
        /// <returns></returns>
        public async Task DeleteSingleMangaBook (MangaBook mangaBook , StorageDeleteOption deleteOption)
        {
            var folder = mangaBook.StorageFolder;
            foreach (var f in MangaFolders)
            {
                if (f.StorageFolder.Path == folder.Path)
                {
                    f.RemoveManga(mangaBook);
                    break;
                }
            }
            await DatabaseController.ReadingInfo_RemoveSingle(mangaBook.StorageFile.Path);

            await mangaBook.StorageFile.DeleteAsync(deleteOption);
        }

        /// <summary>
        /// 事情完成时发生
        /// </summary>
        /// <param name="message"></param>
        public void WorkDone (string message) => WorkDoneEvent?.Invoke(message);

        /// <summary>
        /// 发现错误漫画时引发
        /// </summary>
        /// <param name="manganame"></param>
        public void ErrorMangaEvent (string manganame)
        {
            ErrorZipEvent?.Invoke(manganame);
        }
    }
}