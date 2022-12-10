using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Views;

using Windows.UI.Xaml.Controls;

namespace EroMangaManager.ViewModels
{
    /// <summary>
    /// BookCaseContainer页面的所有数据实例管理器
    /// </summary>
    internal class PageInstancesManager
    {
        internal Dictionary<MangasFolder , Bookcase> Bookcases { get; } = new Dictionary<MangasFolder , Bookcase>();
        internal MainPage MainPage { get; set; }
        internal LibraryPage LibraryPage { set; get; }
        internal ReadPage ReadPage { set; get; }

        /// <summary>
        /// 清除所有页面缓存
        /// TODO
        /// </summary>
        internal void ClearBookcase ()
        {
            Bookcases.Clear();
       foreach(var bookcase in Bookcases)
            {
                var page = bookcase.Value as Page;
                page = null;
            }
        }
    }
}
