using System;
using System.Collections.Generic;
using System.Text;
using EroMangaManager.Database.Tables;
using EroMangaManager.Database.Entities;
using System.Linq;

namespace EroMangaManager.Database.Tools
{
    public static class RecordOperation
    {
        public static void UpdateRecordPosition (this string path, int position, int amount)
        {
            using (Tables.Databases table = new Tables.Databases())
            {
                var record = table.ReadingRecords.Where(n => n.AbsolutePath == path).Single();
                if (record == null)
                {
                    record = new Record() { AbsolutePath = path, PageAmount = amount, ReadingPosition = 0 };
                    table.ReadingRecords.Add(record);
                    table.SaveChanges();
                }
                record.ReadingPosition = position;
                table.SaveChanges();
            }
        }
    }
}