using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace EroMangaDB.Entities
{
    /// <summary> 预设漫画Tag种类 </summary>

    public enum PresetTagCategory
    {
        /// <summary> 全彩Tag </summary>
        全彩,

        /// <summary> 无修Tag </summary>
        无修,

        /// <summary> Download版本 </summary>
        DL版,

        /// <summary> 刊登杂志Tag </summary>
        刊登,

        /// <summary> ComiketMarket展会信息Tag </summary>
        CM展,

        /// <summary> 中译汉化者Tag </summary>
        中译,

        /// <summary> 英译汉化者Tag </summary>
        英译,

        /// <summary> 作者Tag </summary>
        作者,

        /// <summary> 单行本Tag </summary>
        单行本,

        /// <summary> 短篇Tag </summary>
        短篇,
    }
}