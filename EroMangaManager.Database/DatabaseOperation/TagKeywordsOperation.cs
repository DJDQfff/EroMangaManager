using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.EntityFactory;
using EroMangaManager.Database.Helper;
using EroMangaManager.Database.Tables;

namespace EroMangaManager.Database.DatabaseOperation
{
    public static class TagKeywordsOperation
    {
        public static string[] QueryTagKeywords (string tagname)
        {
            Databases databases = new Databases();
            string pieces = databases.TagKeywords.Where(n => n.TagName == tagname).Select(n => n.TagKeywordPieces).Single();
            string[] keywords = pieces.Split('\r');
            return keywords;
        }

        public static async Task AddSingle (string tagname, IEnumerable<string> keywords)
        {
            TagKeywords tagKeywords = TagKeywordsFactory.Creat(tagname, keywords);
            Databases databases = new Databases();
            databases.TagKeywords.Add(tagKeywords);
            await databases.SaveChangesAsync();
        }

        public static async Task UpdateSingle (string tagname, IEnumerable<string> keywords)
        {
            Databases databases = new Databases();
            string pieces = string.Join("\r", keywords);
            TagKeywords tagKeywords = databases.TagKeywords.Single(n => n.TagName == tagname);
            tagKeywords.TagKeywordPieces = pieces;
            databases.Update(tagKeywords);
            await databases.SaveChangesAsync();
        }
    }
}