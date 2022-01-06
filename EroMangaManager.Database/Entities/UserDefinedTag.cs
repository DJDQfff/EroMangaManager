using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EroMangaManager.Database.Entities
{
    public class UserDefinedTag
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> 自定义Tag的名称 </summary>
        public string TagName { set; get; }

        /// <summary>
        /// 自定义Tag的各个片段，每个片段以‘\r’分割，每个片段都是该tag的具体标志之一
        /// </summary>
        public string TagPieces { set; get; }
    }
}