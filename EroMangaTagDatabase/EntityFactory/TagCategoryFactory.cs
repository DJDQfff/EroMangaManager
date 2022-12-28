using System.Collections.Generic;

using EroMangaDB.Entities;

namespace EroMangaDB.EntityFactory
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
        public static TatCategory Creat (string categoryname , IEnumerable<string> keywords)
        {
            string keywordsstring = string.Join("\r" , keywords);
            TatCategory tagKeywords = new TatCategory() { CategoryName = categoryname , Keywords = keywordsstring };
            return tagKeywords;
        }
    }
}