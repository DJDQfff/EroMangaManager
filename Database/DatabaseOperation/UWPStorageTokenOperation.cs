using EroMangaDatabase.Entities;
using System.Linq;

namespace EroMangaDatabase
{
    public partial class BasicController
    {
        /// <summary>
        /// 移除单个UWPToken
        /// </summary>
        /// <param name="path"></param>
        public void UWPStorageToken_RemoveSingle(string path)
        {
            var a = database.UWPAccessIStorages.Single(x => x.Path == path);
            database.Remove(a);
            database.SaveChanges();
        }

        /// <summary>
        /// 添加单个UWPToekn
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <param name="fileoffolder"></param>
        public void UWPStorageToken_AddSingle(string path, string token, bool fileoffolder)
        {
            var access = new UWPAccessIStorage()
            {
                AccessToken = token,
                Path = path,
                IsFileOrFolder = fileoffolder
            };

            database.UWPAccessIStorages.Add(access);
            database.SaveChanges();
        }

        /// <summary>
        /// 获取所有UWPToken
        /// </summary>
        /// <returns></returns>
        public UWPAccessIStorage[] UWPStorageToken_QueryAll()
        {
            var a = database.UWPAccessIStorages.ToArray();
            return a;
        }
    }
}