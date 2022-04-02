using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaTagDatabase.Helper;

using static EroMangaTagDatabase.BasicController;

namespace EroMangaTagDatabase.UpperController
{
    /// <summary> 封装了对ReadingInfo的上层操作 </summary>
    public class ReadingInfoManager : IDisposable
    {
        /// <summary> 从数据库中提取出来，用于查询 </summary>
        public ReadingInfo[] InDatabasereadingInfos { set; get; }

        /// <summary> 要添加到数据库中的新数据 </summary>
        public List<ReadingInfo> NeedAddToDatabase { set; get; } = new List<ReadingInfo>();

        /// <summary> 构造函数 </summary>
        public ReadingInfoManager ()
        {
            InDatabasereadingInfos = DatabaseController.ReadingInfo_QueryAll();
        }

        /// <summary> 获得一个ReadingInf，如果不存在则创建一个 </summary>
        /// <param name="path"> </param>
        /// <returns> </returns>
        public ReadingInfo Get (string path)
        {
            ReadingInfo readingInfo;
            try
            {   //
                readingInfo = InDatabasereadingInfos.Single(n => n.AbsolutePath == path);
            }
            catch (InvalidOperationException)
            {
                readingInfo = ReadingInfoFactory.Creat(path);
                NeedAddToDatabase.Add(readingInfo);
            }
            return readingInfo;
        }

        /// <summary> 把readinginfo保存到数据库 </summary>
        /// <returns> </returns>
        public async Task SaveChanges ()
        {
            await DatabaseController.ReadingInfo_AddMulti(NeedAddToDatabase);
        }

        /// <summary> 释放此实例 </summary>
        public void Dispose ()
        {
            InDatabasereadingInfos = null;
            NeedAddToDatabase.Clear();
            NeedAddToDatabase = null;
        }

        // TODO 改用这个
        public ReadingInfo this[string path]
        {
            set
            {
            }
        }
    }
}