using System;
using System.Collections.Generic;

using Config.Net;

namespace EroMangaManager.UWP.SettingEnums
{
    /// <summary>
    /// 应用设置枚举项，使用是使用ToStrng（）方法转化为string，从ApplicationModel.Resources命名空间中获取应用程序设置
    /// </summary>
    public interface IAppConfig
    {
        /// <summary>
        /// 是否启用图片过滤功能
        /// </summary>
        [Option(DefaultValue = false)]
        bool IsFilterImageOn { set; get; }

        /// <summary>
        /// 删除漫画源文件前是否显示删除确认对话框
        /// </summary>
        [Option(DefaultValue = false)]
        bool WhetherShowDialogBeforeDelete { set; get; }

        /// <summary>
        /// 删除模式，是移动到回收站还是直接磁盘删除
        /// </summary>
        [Option(DefaultValue = false)]
        bool StorageDeleteOption { set; get; }

        /// <summary>
        /// 默认的Bookcase展示的页面
        /// </summary>
        [Option(DefaultValue = null)]
        string DefaultBookcaseFolder { set; get; }

        /// <summary>
        /// 是否在LibraryPage显示空漫画的文件夹
        /// </summary>
        bool IsEmptyFolderShow { set; get; }
        /// <summary>
        /// 添加的文件夹集合
        /// </summary>
        /// <remarks>当前版本的Config.Net对IEnumerable只读，还不能直接写</remarks>
        [Option(DefaultValue =new string[] {})]
        string[] LibraryFolders { get; set; }
    }
}