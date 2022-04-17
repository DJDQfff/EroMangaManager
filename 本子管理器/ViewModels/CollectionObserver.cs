using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using EroMangaTagDatabase;
using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaManager.Models;
using static EroMangaTagDatabase.BasicController;
using Windows.Storage;

using static Windows.Storage.AccessCache.StorageApplicationPermissions;

namespace EroMangaManager.ViewModels
{
    public class CollectionObserver
    {
        public event Action<string> ErrorZipEvent;

        /// <summary> 存放zip文件的文件夹 </summary>
        public ObservableCollection<StorageFolder> FolderList { set; get; } = new ObservableCollection<StorageFolder>();

        /// <summary> 各漫画zip </summary>
        public ObservableCollection<MangaBook> MangaList { set; get; } = new ObservableCollection<MangaBook>();

        /// <summary> 流的内容不是 zip 存档格式。 </summary>
        public ObservableCollection<MangaBook> ErrorZip { set; get; } = new ObservableCollection<MangaBook>();

        public CollectionObserver ()
        {
            Initialize();
        }

        /// <summary> 初始化文件夹 </summary>
        public async void Initialize ()
        {
            FolderList.Clear();
            MangaList.Clear();//初始化清零
            ErrorZip.Clear();

            var folders = FutureAccessList.Entries;
            foreach (var item in folders)
            {
                try
                {
                    StorageFolder storageFolder = await FutureAccessList.GetFolderAsync(item.Token);
                    FolderList.Add(storageFolder);
                }
                catch (Exception)
                {
                    // 文件夹被移除，找不到，
                }
            }
            foreach (var folder in FolderList)
            {
                await PickMangasInFolder(folder);
            }
        }

        /// <summary>
        /// 添加文件夹，并添加文件夹及下属漫画漫画到集合 1.添加改文件夹到系统API
        /// 2.添加文件夹到FolderList 3.添加文件夹下属漫画到MangaList
        /// </summary>
        /// <param name="folder"> </param>
        /// <returns> </returns>
        public async Task AddFolder (StorageFolder folder)
        {
            FutureAccessList.Add(folder);

            var query = FolderList.Where(n => n.Path == folder.Path).Count();

            if (query == 0)
            {
                FolderList.Add(folder);
                await PickMangasInFolder(folder);
            }
        }

        /// <summary>
        /// 移除文件夹，并从集合中移除文件夹及下属漫画 1.从系统API中移除
        /// 2.从FolderList里移除 3.从MangaList里移除文件夹下属漫画
        /// </summary>
        /// <param name="folder"> </param>
        public void RemoveFolder (StorageFolder folder)
        {
            FolderList.Remove(folder);      // 从访问列表Model里移除

            string token = FutureAccessList.Add(folder);        // 从系统存储api里移除
            FutureAccessList.Remove(token);

            for (int i = MangaList.Count - 1; i >= 0; i--)      // 从Model中移除漫画
            {
                if (MangaList[i].StorageFolder == folder)
                {
                    MangaList.Remove(MangaList[i]);
                }
            }
        }

        /// <summary> 提取文件夹的下属漫画到MangaList </summary>
        /// <param name="storageFolder"> </param>
        /// <returns> </returns>
        private async Task PickMangasInFolder (StorageFolder storageFolder)
        {
            var files = await storageFolder.GetFilesAsync();
            ReadingInfo[] tags = DatabaseController.ReadingInfo_QueryAll();

            List<ReadingInfo> add = new List<ReadingInfo>();
            // TODO：这里如果使用 list<task> 的话，会出bug
            // 普通的遍历添加反而不会出现bug bug 名称：已为另一线程调用
            for (int i = 0; i < files.Count; i++)
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

                MangaBook manga = new MangaBook(storageFile, storageFolder, readingInfo);

                MangaList.Add(manga);

                await manga.TryCreatCoverFileAsync(storageFile);

                await manga.SetCover();
            }

            await DatabaseController.ReadingInfo_AddMulti(add);
        }

        #region 弃用

        /// <summary> 读取数据库，将指定文件夹下的 MangaTag 移除 </summary>
        /// <param name="storageFolder"> </param>
        /// <returns> </returns>
        private async Task RemoveMultiTagsFromDatabase (StorageFolder storageFolder)
        {
            var files = (await storageFolder.GetFilesAsync()).Select(n => n.Path).ToArray();
            await DatabaseController.MangaTag_RemoveMulti(files);
        }

        /// <summary> 从数据库中移除指定漫画的 MangaTag </summary>
        /// <param name="mangaBook"> </param>
        /// <returns> </returns>
        public async Task DeleteSingleMangaBook (MangaBook mangaBook)
        {
            await mangaBook.StorageFile.DeleteAsync();
            MangaList.Remove(mangaBook);
            await DatabaseController.MangaTag_RemoveSingle(mangaBook.StorageFile.Path);
        }

        #endregion 弃用
    }
}