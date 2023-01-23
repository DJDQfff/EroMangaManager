using System.Collections.Generic;

using EroMangaManager.Views.MainPageChildPages;

using Windows.UI.Xaml.Controls;

namespace EroMangaManager.ViewModels
{
    /// <summary>
    /// BookCaseContainer页面的所有数据实例管理器
    /// </summary>
    internal class PageInstancesManager
    {
        internal MainPage MainPage { get; set; }
        internal LibraryPage LibraryPage { set; get; }
        internal ReadPage ReadPage { set; get; }

    }
}