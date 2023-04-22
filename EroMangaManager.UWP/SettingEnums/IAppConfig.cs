namespace EroMangaManager.UWP.SettingEnums
{
    /// <summary>
    /// 应用设置枚举项，使用是使用ToStrng（）方法转化为string，从ApplicationModel.Resources命名空间中获取应用程序设置
    /// </summary>
    public enum General
    {
        /// <summary>
        /// 是否启用图片过滤功能
        /// </summary>
        IsFilterImageOn,

        /// <summary>
        /// 删除漫画源文件前是否显示删除确认对话框
        /// </summary>
        WhetherShowDialogBeforeDelete,

        /// <summary>
        /// 删除模式，是移动到回收站还是直接磁盘删除
        /// </summary>
        StorageFileDeleteOption,

        /// <summary>
        /// 默认的Bookcase展示的页面
        /// </summary>
        DefaultBookcaseFolder,

        /// <summary>
        /// 是否在LibraryPage显示空漫画的文件夹
        /// </summary>
        IsEmptyFolderShow,

        /// <summary>
        /// 添加的文件夹集合
        /// </summary>
        /// <remarks>当前版本的Config.Net对IEnumerable只读，还不能直接写</remarks>
        LibraryFolders,
    }
}