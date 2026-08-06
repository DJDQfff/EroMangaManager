using System.Collections.Generic;
using Database.Entities;

namespace Database.EntityFactory
{
    /// <summary>
    /// TagCategory工厂方法
    /// </summary>
    public static class TagCategoryFactory
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="categoryname"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static TagCategory Creat(string categoryname, IEnumerable<string> keywords)
        {
            string keywordstring = keywords switch
            {
                not null => string.Join(Environment.NewLine, keywords),
                _ => string.Empty,
            };
            TagCategory tagKeywords = new()
            {
                CategoryName = categoryname,
                Keywords = keywordstring,
            };
            return tagKeywords;
        }
    }
}
