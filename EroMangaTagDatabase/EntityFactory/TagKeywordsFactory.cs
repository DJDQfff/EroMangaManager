using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaDB.Entities;

namespace EroMangaDB.EntityFactory
{
    /// <summary>
    /// UniqueTagInRelation工厂
    /// </summary>
    public static class TagKeywordsFactory
    {
        /// <summary>
        /// 创建唯一关联tag
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public static UniqueTagInRelation Creat (string tagname, IEnumerable<string> keywords)
        {
            string keywordsstring = string.Join("\r", keywords);
            UniqueTagInRelation tagKeywords = new UniqueTagInRelation() { TagName = tagname, Keywords = keywordsstring };
            return tagKeywords;
        }
    }
}