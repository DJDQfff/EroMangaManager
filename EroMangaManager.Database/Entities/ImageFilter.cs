using System;
using System.Collections.Generic;
using System.Text;

namespace EroMangaManager.Database.Entities
{
    /// <summary> 数据库中，Hash 是唯一的（算法保证唯一），length 不唯一 </summary>
    public class ImageFilter
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> 图片流的sha256哈希值 </summary>
        public string Hash { set; get; }

        /// <summary> ZipEntry 的解压后流大小 </summary>
        public long ZipEntryLength { set; get; }
    }
}