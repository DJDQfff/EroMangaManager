using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.Helper;

namespace EroMangaTagDatabase
{
    public partial class Controller
    {
        /// <summary> </summary>
        /// <param name="tags"> </param>
        /// <returns>
        /// 一个字典，第一项为本子标签，第二项为对应的TagName（如没有则为空字符串）
        /// </returns>
        public Dictionary<string, string> MatchTag (IEnumerable<string> tags)
        {
            var dictionaries = DatabaseController.TagKeywords_QueryAll();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            foreach (var tag in tags)
            {
                string b = null;
                foreach (var d in dictionaries)
                {
                    if (d.Value.Contains(tag))
                    {
                        b = d.Key;
                        break;
                    }
                }
                // 未在数据库中找到，则传入一个null
                string key = (b is null) ? null : b;
                keyValuePairs.Add(tag, key);
            }
            return keyValuePairs;
        }

        /// <summary> 保存数据库更改 </summary>
        /// <returns> </returns>
        public async Task SaveChanges ()
        {
            await database.SaveChangesAsync();
        }
    }
}