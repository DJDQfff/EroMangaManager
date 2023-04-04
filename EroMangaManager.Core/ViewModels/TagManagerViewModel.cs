using System.Collections.Generic;
using System.Linq;

namespace EroMangaManager.Core.ViewModels
{
    public class TagManagerViewModel
    {
        public List<string> AllTags;
        private List<string> originTags;
        public TagManagerViewModel(IEnumerable<string[]> strings)
        {
            
            var tempalltags = new List<string>();
            foreach (var manga in strings)
            {
                tempalltags.AddRange(manga);
            }

            originTags = tempalltags.Distinct().ToList();
            AllTags = originTags;
        }

        public void Remove(string tag)
        {
            AllTags.Remove(tag);
        }

        public void Initial()
        {
            AllTags.Clear();
            AllTags.AddRange(originTags);
        }
        public List<string> Search(string query)
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