using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EroMangaDB
{
    public partial class BasicController
    {
        /// <summary> 查询数据库，获取Tag键值对的信息 </summary>
        /// <param name="tags"> </param>
        /// <returns>
        /// 一个字典，第一项为本子标签，第二项为对应的TagName（如没有则为空字符串）
        /// </returns>
        public Dictionary<string, string> MatchTag(IEnumerable<string> tags)
        {
            var dictionaries = DatabaseController.TagCategory_QueryAll();

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            tags = tags.Distinct().ToArray();           // 去重

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
        public async Task SaveChanges()
        {
            await database.SaveChangesAsync();
        }
    }
}