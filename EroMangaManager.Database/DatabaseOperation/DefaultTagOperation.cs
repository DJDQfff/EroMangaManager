using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EroMangaManager.Database.DatabaseOperation
{
    public static class DefaultTagOperation
    {
        public static async Task AddTag (DefaultTagBasementType basek, string tag)
        {
            Entities.DefaultTagFilter defaultTagFilter = new Entities.DefaultTagFilter() { Content = tag };
            Tables.Databases databases = new Tables.Databases();

            var @base = SelectDataBase(basek, databases);
            @base.Add(defaultTagFilter);
            await databases.SaveChangesAsync();
        }

        public static string[] QueryTags (DefaultTagBasementType basek)
        {
            Tables.Databases databases = new Tables.Databases();
            var @base = SelectDataBase(basek, databases);
            var vs = @base.Select(n => n.Content).ToArray();
            return vs;
        }

        public static DbSet<Entities.DefaultTagFilter> SelectDataBase (DefaultTagBasementType basek, Tables.Databases databases)
        {
            DbSet<Entities.DefaultTagFilter> result = default;
            switch (basek)
            {
                case DefaultTagBasementType.AuthorTagsFromShared:
                    result = databases.AuthorTagsFromShared;
                    break;

                case DefaultTagBasementType.AuthorTagsFromUser:
                    result = databases.AuthorTagsFromUser;
                    break;

                case DefaultTagBasementType.TranslatorTagsFromShared:
                    result = databases.TranslatorTagsFromShared;
                    break;

                case DefaultTagBasementType.TranslatorTagsFromUser:
                    result = databases.TranslatorTagsFromUser;
                    break;
            }
            return result;
        }
    }
}