using System.Collections.Generic;
using System.Linq;

namespace EroMangaManager.Core.ViewModels
{
    public class TagManagerViewModel
    {
        /// <summary>
        /// 对外公开的所有项
        /// </summary>
        public List<string> AllTags { set; get; }

        /// <summary>
        /// 选中项
        /// </summary>
        public List<string> SelectedTags = new List<string>();

        /// <summary>
        /// 数据源
        /// </summary>
        private readonly IEnumerable<string> originTags;

        /// <summary>
        /// 隐藏起来的项
        /// </summary>
        private readonly List<string> hidedTags = new List<string>();

        public TagManagerViewModel(IEnumerable<string[]> strings)
        {
            var tempalltags = new List<string>();
            foreach (var manga in strings)
            {
                tempalltags.AddRange(manga);
            }

            originTags = tempalltags.Distinct();
            AllTags = new List<string>(originTags);
        }

        public void HideTag(string tag)
        {
            if (AllTags.Remove(tag))
            {
                hidedTags.Add(tag);
            }
        }

        public void CancelHideTag(string tag)
        {
            if (hidedTags.Remove(tag))
            {
                AllTags.Add(tag);
            }
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