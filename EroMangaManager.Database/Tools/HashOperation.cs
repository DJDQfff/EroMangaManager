using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Tables;

namespace EroMangaManager.Database.Tools
{
    public static class HashOperation
    {
        public static Tables.Database table = new Tables.Database();

        public static int LengthConditionCount (long length)
        {
            var query = table.imageHashes.Where(n => n.ZipEntryLength == length ).Count();
            return query;
        }

        public static int HashConditionCount (string hash)
        {
            var query = 6;
            query = table.imageHashes.Where(n => n.Hash == hash).Count();
            return query;
        }

        public static void Add (string hash, long length)
        {
            ImageHash imageHash = new ImageHash()
            {
                Hash = hash,
                ZipEntryLength = length,
            };
            table.Add(imageHash);
            table.SaveChanges();
        }

        public static void Remove (string hash)
        {
            var h = table.imageHashes.Where(n => n.Hash == hash).Single();
            table.Remove(h);
            table.SaveChanges();
        }
    }
}