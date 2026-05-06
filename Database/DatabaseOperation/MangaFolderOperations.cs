using EroMangaDatabase.Entities;
using System.Linq;

namespace EroMangaDatabase
{
    public partial class BasicController
    {
        /// <summary>
        /// 返回所有文件夹路径
        /// </summary>
        /// <returns></returns>
        public string[] MangaFolder_GetAllPaths()
        {
            var query = database.MangaFolders.Select(x => x.Path).ToArray();
            return query;
        }

        /// <summary>
        /// 添加单个文件夹，如果数据库已存在相同路径，则跳过。
        /// 添加成功则返回true
        /// </summary>
        /// <param name="path"></param>
        public bool MangaFolder_AddSingle(string path)
        {
            var a = database.MangaFolders.FirstOrDefault(x => x.Path == path);
            if (a is null)
            {
                var folder = new MangaFolder() { Path = path };
                database.MangaFolders.Add(folder);
                database.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 移除单个文件夹
        /// </summary>
        /// <param name="path"></param>
        public void MangaFolder_RemoveSingle(string path)
        {
            var folder = database.MangaFolders.SingleOrDefault(x => x.Path == path);
            if (folder != null)
            {
                database.Remove(folder);
                database.SaveChanges();
            }
        }
    }
}