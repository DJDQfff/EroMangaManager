using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EroMangaDB.Entities
{
    /// <summary> 
    /// 唯一标签。
    /// 为什么这么设计：
    /// 如：初音未来、MIKU这两个tag可能看起来是来个不同tsg，但，其实是同一个tag。
    /// 所以这两个都是tag都作为一个TagKeywords的两个TagKeywords，同时指定一个TagName作为唯一名</summary>
    public class RealUniqueTag : IDatabaseID
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