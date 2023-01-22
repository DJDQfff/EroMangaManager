using System;
using System.Collections.Generic;
using System.Linq;

namespace EroMangaDB.Helper
{
    /// <summary>
    /// TagKeyWords帮助类
    /// </summary>
    public static class TagKeywordsDictionaryHelper
    {
        /// <summary>
        /// 返回字典中包含查找项的
        /// </summary>
        /// <param name="keyValuePairs"></param>
        /// <param name="searchkeyword"></param>
        /// <returns>包含此项的key，没有则为null</returns>
        public static string Searchkeyword (this Dictionary<string , string[]> keyValuePairs , string searchkeyword)
        {
            foreach (var pair in keyValuePairs)
            {
                if (pair.Value.Contains(searchkeyword))
                {
                    return pair.Key;
                }
            }
            return null;
        }
    }
}