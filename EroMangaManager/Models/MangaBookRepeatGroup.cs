using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EroMangaManager.Models
{
    internal class MangaBookRepeatGroup : RepeatGroup<string , MangaBook>
    {
        public MangaBookRepeatGroup (IGrouping<string , MangaBook> _files) : base(_files)
        {
        }
    }
}
