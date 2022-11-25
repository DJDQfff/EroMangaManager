using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaDB.Entities;

namespace EroMangaDB.EntityFactory
{
    public static class TagKeywordsFactory
    {
        public static UniqueTagInRelation Creat (string tagname, IEnumerable<string> keywords)
        {
            string keywordsstring = string.Join("\r", keywords);
            UniqueTagInRelation tagKeywords = new UniqueTagInRelation() { TagName = tagname, Keywords = keywordsstring };
            return tagKeywords;
        }
    }
}