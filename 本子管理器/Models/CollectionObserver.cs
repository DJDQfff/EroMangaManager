using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Storage;
using EroMangaManager.Database.Entities;
using EroMangaManager.Database.DatabaseOperation;
using EroMangaManager.Database.Utility;
using static Windows.Storage.AccessCache.StorageApplicationPermissions;

namespace EroMangaManager.Models
{
    public class CollectionObserver
    {
        public ObservableCollection<StorageFolder> FolderList { set; get; } = new ObservableCollection<StorageFolder>();
        public ObservableCollection<MangaBook> MangaList { set; get; } = new ObservableCollection<MangaBook>();

        public CollectionObserver ()
        {
            Initialize();
        }

        /// <summary> 初始化文件夹 </summary>
        public async void Initialize ()
        {
            FolderList.Clear();
            MangaList.Clear();//初始化清零

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

        public async void AddFolder (StorageFolder folder)
        {
            FutureAccessList.Add(folder);

            var query = FolderList.Where(n => n.Path == folder.Path).Count();

            if (query == 0)
            {
                FolderList.Add(folder);
                await SaveTagsToDatabase(folder);
                await PickMangasInFolder(folder);
            }
        }

        public async Task RemoveFolder (StorageFolder folder)
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
            await RemoveTagsFromDatabase(folder);                           // 从数据库中移除
        }

        private async Task PickMangasInFolder (StorageFolder storageFolder)
        {
            var files = await storageFolder.GetFilesAsync();
            MangaTag[] tags = TagOperation.QuertTags(storageFolder.Path);

            // TODO：这里如果使用 list<task> 的话，会出bug
            // 普通的遍历添加反而不会出现bug
            // bug 名称：已为另一线程调用
            for (int i = 0; i < files.Count; i++)
            {
                MangaTag mangaTag;
                try
                {   // 文件夹内容与数据库内容不同
                    mangaTag = tags.Single(n => n.AbsolutePath == files[i].Path);
                }
                catch (InvalidOperationException)
                {
                    mangaTag = MangaTagFactory.Creat(files[i].Path);
                    await TagOperation.SaveTag(mangaTag);
                }
                MangaBook manga = new MangaBook(files[i], storageFolder, mangaTag);

                MangaList.Add(manga);         // 多线程下，对list进行写操作，会出bug

                await manga.EnsureCoverFile();

                await manga.SetCover();
            }
        }

        private async Task SaveTagsToDatabase (StorageFolder storageFolder)
        {
            var files = await storageFolder.GetFilesAsync();

            List<MangaTag> list = new List<MangaTag>();
            foreach (var file in files)
            {
                MangaTag mangaTag = MangaTagFactory.Creat(file.Path);
                list.Add(mangaTag);
            }
            await TagOperation.SaveTags(list);
        }

        /// <summary> 从数据库中移除传入文件夹下的文件 </summary>
        /// <param name="storageFolder"> </param>
        /// <returns> </returns>
        private async Task RemoveTagsFromDatabase (StorageFolder storageFolder)
        {
            var files = (await storageFolder.GetFilesAsync()).Select(n => n.Path).ToArray();
            await TagOperation.RemoveTags(files);
        }

        public async Task DeleteSingleMangaBook (MangaBook mangaBook)
        {
            await mangaBook.StorageFile.DeleteAsync();
            MangaList.Remove(mangaBook);
            await TagOperation.RemoveTag(mangaBook.StorageFile.Path);
        }
    }
}