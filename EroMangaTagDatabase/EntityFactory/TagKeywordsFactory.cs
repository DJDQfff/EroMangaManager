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
        public static RealUniqueTag Creat (string tagname, IEnumerable<string> keywords)
        {
            string keywordsstring = string.Join("\r", keywords);
            RealUniqueTag tagKeywords = new RealUniqueTag() { TagName = tagname, Keywords = keywordsstring };
            return tagKeywords;
        }
    }
}