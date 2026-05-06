using EroMangaDatabase.Entities;
using System.Collections.Generic;

namespace EroMangaDatabase.EntityFactory
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
                null => string.Join("\r", keywords),
                _ => string.Empty,
            };
            TagCategory tagKeywords =
                new() { CategoryName = categoryname, Keywords = keywordstring };
            return tagKeywords;
        }
    }
}