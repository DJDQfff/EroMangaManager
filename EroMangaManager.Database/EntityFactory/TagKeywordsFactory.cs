using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaTagDatabase.Entities;

namespace EroMangaTagDatabase.EntityFactory
{
    public static class TagKeywordsFactory
    {
        public static TagKeywords Creat (string tagname, IEnumerable<string> keywords)
        {
            string pieces = string.Join("\r", keywords);
            TagKeywords tagKeywords = new TagKeywords() { TagName = tagname, TagKeywordPieces = pieces };
            return tagKeywords;
        }
    }
}