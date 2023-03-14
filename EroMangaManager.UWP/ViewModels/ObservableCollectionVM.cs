﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using EroMangaManager.UWP.Models;
using EroMangaManager.Core.ViewModels;
using EroMangaManager.Core.Models;
using Windows.Storage;

using static EroMangaDB.BasicController;
using static Windows.Storage.AccessCache.StorageApplicationPermissions;
using MyLibrary.UWP;

namespace EroMangaManager.UWP.ViewModels
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
        internal List<string> StorageFolders => MangaFolders.Select(n => n.FolderPath).ToList();

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

        /// <summary>
        /// MangasFolder是否正在更新，有任意一个是则返回true
        /// </summary>
        public bool IsContentInitializing => MangaFolders.Any((x) => x.IsInitialing == true);

        /// <summary>流的内容不是 zip 存档格式。</summary>
        internal ObservableCollection<MangaBook> NonZipList { get; } = new ObservableCollection<MangaBook>();

        /// <summary>ViewModel初始化</summary>
        public async Task Initialize (IEnumerable<StorageFolder> storageFolders)
        {
            MangaFolders.Clear();
            NonZipList.Clear();

            //TODO 现在这个是顺序执行，试试多线程方法，加快速度
            foreach (var folder in storageFolders)
            {
                MangasFolder mangasFolder =new MangasFolder(folder.Path);
                MangaFolders.Add(mangasFolder);
            };

            foreach (var folder in MangaFolders)
            {
                var storage = App.Current.storageItemManager.GetStorageFolder(folder.FolderPath);
                await ModelFactory.InitialMangasFolder(folder , storage);
            }
            // TODO 用这样的话，所有的都卡着不动，这是不是线程死锁？
            List<Task> tasks = new List<Task>();
            //foreach (var folder in MangaFolders)
            //{
            //    var f = folder;
            //    Task task = new Task(async () => await f.Initial());
            //    tasks.Add(task);
            //}
            //await Task.WhenAll(tasks);

            //foreach(var f in MangaFolders)
            //{
            //    Thread thread = new Thread(async () => await f.Initial());
            //    thread.Start();
            //}
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
                MangasFolder mangasFolder = new MangasFolder(folder.Path);
                MangaFolders.Add(mangasFolder);
                await ModelFactory.InitialMangasFolder(mangasFolder , folder);
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

            App.Current.storageItemManager.RemoveToken(mangasfolder.FolderPath);

        }

        /// <summary>从数据库中移除指定漫画的 MangaTag</summary>
        /// <param name="mangaBook"></param>
        /// <param name="deleteOption">删除模式</param>
        /// <returns></returns>
        public async Task DeleteSingleMangaBook (MangaBook mangaBook , StorageDeleteOption deleteOption)
        {
            await App.Current.storageItemManager.DeleteStorageFile( mangaBook.FilePath, StorageDeleteOption.Default);

            // TODO 这里可以在设置里添加一个“是否删除阅读记录”
            await DatabaseController.ReadingInfo_RemoveSingle(mangaBook.FilePath);


            var file = await App.Current.storageItemManager.GetStorageFile(mangaBook.FilePath);
            await file.DeleteAsync(deleteOption);
        }

        /// <summary>
        /// 重命名漫画
        /// </summary>
        /// <param name="mangaBook"></param>
        /// <param name="newdisplayname"></param>
        /// <returns></returns>
        public async Task ReNameSingleMangaBook (MangaBook mangaBook , string newdisplayname)
        {
            var name = mangaBook.FilePath;
            var newname = newdisplayname + ".zip";

            await App.Current.storageItemManager.RenameStorageFile(name, newdisplayname);

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