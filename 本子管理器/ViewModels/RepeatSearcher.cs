using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;

namespace EroMangaManager.ViewModels
{
    /// <summary> 检查是否有本子重复 </summary>
    internal class RepeatSearcher
    {
    }

    public class MangaImageHash
    {
        private StorageFile StorageFile { get; set; }
        public string[] Hashes { set; get; }

        public void Set ()
        {
        }
    }
}