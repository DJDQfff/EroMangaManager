using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaTagDatabase.Helper;
using EroMangaTagDatabase.Tables;

namespace EroMangaTagDatabase.DatabaseOperation
{
    public static class TagKeywordsOperation
    {
        public static string[] QueryTagKeywords (string tagname)
        {
            Database databases = new Database();
            string pieces = databases.TagKeywords.Where(n => n.TagName == tagname).Select(n => n.TagKeywordPieces).Single();
            string[] keywords = pieces.Split('\r');
            databases.Dispose();

            return keywords;
        }

        public static async Task AddSingle (string tagname, IEnumerable<string> keywords)
        {
            TagKeywords tagKeywords = TagKeywordsFactory.Creat(tagname, keywords);
            Database databases = new Database();
            databases.TagKeywords.Add(tagKeywords);
            await databases.SaveChangesAsync();
            databases.Dispose();
        }

        public static async Task UpdateAppendSingle (string tagname, string piece)
        {
            Database databases = new Database();
            TagKeywords tagKeywords = databases.TagKeywords.Single(n => n.TagName == tagname);
            tagKeywords.TagKeywordPieces += "\r" + piece;
            databases.Update(tagKeywords);
            await databases.SaveChangesAsync();
            databases.Dispose();
        }

        public static async Task UpdateSingle (string tagname, IEnumerable<string> keywords)
        {
            Database databases = new Database();
            string pieces = string.Join("\r", keywords);
            TagKeywords tagKeywords = databases.TagKeywords.Single(n => n.TagName == tagname);
            tagKeywords.TagKeywordPieces = pieces;
            databases.Update(tagKeywords);
            await databases.SaveChangesAsync();
            databases.Dispose();
        }
    }
}