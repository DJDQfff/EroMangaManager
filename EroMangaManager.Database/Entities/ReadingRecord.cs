using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EroMangaManager.Database.Entities
{
    public class ReadingRecord
    {
        /// <summary> 漫画总页数 </summary>
        public int PageAmount { get; set; }

        /// <summary> 当前阅读位置（进度） </summary>
        public int ReadingPosition { get; set; }

    }
}
