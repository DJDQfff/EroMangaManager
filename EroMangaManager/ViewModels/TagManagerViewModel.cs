using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Services;

namespace EroMangaManager.ViewModels
{
    internal class TagManagerViewModel
    {
        public IEnumerable<string> AllTags;

        public TagManagerViewModel (IEnumerable<string[]> strings)
        {
            var tempalltags = new List<string>();
            foreach (var manga in strings)
            {             
                tempalltags.AddRange(manga);
            }

            AllTags = tempalltags.Distinct();
        }

        public List<string> Search (string query)
        {
            var temptags = new List<string>();
            foreach (var x in AllTags)
            {
                if (x.Contains(query))
                {
                    temptags.Add(x);
                }
            };

            return temptags;
        }


    }
}
