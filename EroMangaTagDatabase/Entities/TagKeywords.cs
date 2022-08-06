using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EroMangaTagDatabase.Entities
{
    /// <summary> 标签键值对，含有成员TagName，KeyWords </summary>
    public class TagKeywords : IDatabaseID
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> 自定义Tag的名称 </summary>
        public string TagName { set; get; }

        /// <summary>
        /// 自定义Tag的各个关键字，每个关键字以‘\r’分割，每个关键字都是该tag的识别标志之一
        /// 第一个：对外显示的
        /// </summary>
        public string Keywords { set; get; }
    }
}