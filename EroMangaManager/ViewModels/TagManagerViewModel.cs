using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Services;

namespace EroMangaManager.ViewModels
{
    internal class TagManagerViewModel
    {
       public Dictionary<string , string[]> GetCategory ()
        {
            return EroMangaDB.BasicController.DatabaseController.TagCategory_QueryAll ();
        }

    }
}
