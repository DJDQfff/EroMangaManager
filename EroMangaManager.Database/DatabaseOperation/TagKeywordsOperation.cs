using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EroMangaTagDatabase.DatabaseOperation.DatabaseController;
using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaTagDatabase.Helper;
using EroMangaTagDatabase.Tables;

namespace EroMangaTagDatabase.DatabaseOperation
{
    public partial class DatabaseController
    {
        public string[][] TagKeywords_QueryAll ()
        {
            var allpieces = database.TagKeywords.Select(n => n.TagKeywordPieces).ToArray();

            List<string[]> vs = new List<string[]>();
            foreach (var pieces in allpieces)
            {
                string[] vs1 = pieces.Split('\r');
                vs.Add(vs1);
            }
            string[][] vs2 = vs.ToArray();
            return vs2;
        }

        public string[] TagKeywords_QuerySingle (string tagname)
        {
            string pieces = database.TagKeywords.Where(n => n.TagName == tagname).Select(n => n.TagKeywordPieces).Single();
            string[] keywords = pieces.Split('\r');

            return keywords;
        }

        public async Task TagKeywords_AddSingle (string tagname, IEnumerable<string> keywords)
        {
            TagKeywords tagKeywords = TagKeywordsFactory.Creat(tagname, keywords);
            database.TagKeywords.Add(tagKeywords);
            await database.SaveChangesAsync();
        }

        public async Task TagKeywords_UpdateAppendSingle (string tagname, string piece)
        {
            TagKeywords tagKeywords = database.TagKeywords.Single(n => n.TagName == tagname);
            tagKeywords.TagKeywordPieces += "\r" + piece;
            database.Update(tagKeywords);
            await database.SaveChangesAsync();
        }

        public async Task TagKeywords_UpdateSingle (string tagname, IEnumerable<string> keywords)
        {
            string pieces = string.Join("\r", keywords);
            TagKeywords tagKeywords = database.TagKeywords.Single(n => n.TagName == tagname);
            tagKeywords.TagKeywordPieces = pieces;
            database.Update(tagKeywords);
            await database.SaveChangesAsync();
        }
    }
}