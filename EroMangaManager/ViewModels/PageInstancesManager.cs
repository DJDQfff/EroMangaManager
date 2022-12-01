using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Views;

namespace EroMangaManager.ViewModels
{
    internal class PageInstancesManager
    {
        internal Dictionary<MangasFolder , Bookcase> Bookcases { get; } = new Dictionary<MangasFolder , Bookcase>();
        internal MainPage MainPage { get; set; }
        internal LibraryPage LibraryPage { set; get; }
        internal ReadPage ReadPage { set; get; }

        internal void ClearBookcase ()
        {
            Bookcases.Clear();
        }
    }
}
