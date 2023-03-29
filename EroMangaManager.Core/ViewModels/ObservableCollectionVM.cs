using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using EroMangaManager.Core.Models;

namespace EroMangaManager.Core.ViewModels
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

        public ObservableCollection<MangasFolder> MangaFolders { get; } = new ObservableCollection<MangasFolder>();

        /// <summary>存放zip文件的文件夹</summary>
        public List<string> StorageFolders => MangaFolders.Select(n => n.FolderPath).ToList();

        /// <summary>各漫画zip</summary>
        ///
        public List<MangaBook> MangaList
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

        /// <summary>
        /// MangasFolder是否正在更新，有任意一个是则返回true
        /// </summary>
        public bool IsContentInitializing => MangaFolders.Any((x) => x.IsInitialing == true);



        /// <summary>
        /// 添加文件夹，并添加文件夹及下属漫画漫画到集合
        /// 1.添加改文件夹到系统API
        /// 2.添加文件夹到FolderList
        /// 3.添加文件夹下属漫画到MangaList
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        public MangasFolder AddFolder(string path)
        {
            MangasFolder mangasFolder = new MangasFolder(path);

            MangaFolders.Add(mangasFolder);

            return mangasFolder;
        }

        /// <summary>
        /// 移除文件夹，并从集合中移除文件夹及下属漫画 （只移除，不删除）
        /// 1.从系统API中移除
        /// 2.从FolderList里移除
        /// 3.从MangaList里移除文件夹下属漫画
        /// </summary>
        public void RemoveFolder(MangasFolder mangasfolder)
        {
            MangaFolders.Remove(mangasfolder);
        }
        public void RemoveManga(MangaBook mangaBook)
        {
            string folderpath = mangaBook.FolderPath;
            var folder = MangaFolders.Single(x => x.FolderPath == folderpath);
            folder.RemoveManga(mangaBook);
        }

        /// <summary>
        /// 事情完成时发生
        /// </summary>
        /// <param name="message"></param>
        public void WorkDone(string message) => WorkDoneEvent?.Invoke(message);

        /// <summary>
        /// 发现错误漫画时引发
        /// </summary>
        /// <param name="manganame"></param>
        public void ErrorMangaEvent(string manganame)
        {
            ErrorZipEvent?.Invoke(manganame);
        }
    }
}