using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace EroMangaManager.Models
{

    internal class RepeatMangaBookGroup : RepeatItems.RepeatItemGroup<string,MangaBook>
    {
        public RepeatMangaBookGroup (IGrouping<string , MangaBook> _files) : base(_files)
        {
        }
    }

}
