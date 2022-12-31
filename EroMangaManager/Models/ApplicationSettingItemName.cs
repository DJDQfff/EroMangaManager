﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EroMangaManager.Models
{
    /// <summary>
    /// 应用设置枚举项，使用是使用ToStrng（）方法转化为string，从ApplicationModel.Resources命名空间中获取应用程序设置
    /// </summary>
    internal enum ApplicationSettingItemName
    {
        /// <summary>
        /// 是否弃用图片过滤功能
        /// </summary>
        IsFilterImageOn,
        /// <summary>
        /// 删除漫画源文件前是否显示删除确认对话框
        /// </summary>
        WhetherShowDialogBeforeDelete,
        /// <summary>
        /// 删除模式，是移动到回收站还是直接磁盘删除
        /// </summary>
        StorageDeleteOption
    }
}
