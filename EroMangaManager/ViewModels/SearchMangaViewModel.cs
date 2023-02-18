using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EroMangaManager.ViewModels
{
    internal class SearchMangaViewModel
    {
        public IEnumerable<string> alltags;
        public SearchMangaViewModel()
        {
            var mangas = App.Current.collectionObserver.MangaList;
            var tempalltags=new List<string>();
            foreach(var manga in mangas)
            {
                var ts = manga.MangaTags;
                tempalltags.AddRange(ts);
            }

            alltags = tempalltags.Distinct();
        }

        public List<string> Search (string query)
        {
            var temptags = new List<string>();
            foreach(var x in alltags)
            {
                if(x.Contains(query))
                {
                    temptags.Add(x);
                }
            };

           return temptags;
        }
    }
}
