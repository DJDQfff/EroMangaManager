using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Models;

namespace EroMangaManager.ViewModels
{
    /// <summary> 管理无效的漫画文件 </summary>
    public class InvalidZipManager
    {
        public ObservableCollection<MangaBook> mangaBooks { set; get; } = new ObservableCollection<MangaBook>();
    }
}