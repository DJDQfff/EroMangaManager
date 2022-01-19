using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Database.DatabaseOperation;
using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Tables;

using Microsoft.EntityFrameworkCore;

namespace EroMangaManager.Database.DatabaseOperation
{
    public static class DatabaseInitialize
    {
        /// <summary>
        /// 数据库迁移，并初始化默认数据
        /// </summary>
        public static async Task InitializeDefaultData ()
        {
            using (Databases databases = new Databases())
            {
                // 版本迁移
                databases.Database.Migrate();

                // 初始化数据
                List<string>[] vs = new List<string>[]
                {
                new List<string> { "fullColorTags", "全彩" },
                new List<string>{ "nonMosaicTags","无修", "無修", "无码","無码" },
                new List<string> { "downloadversionTags", "DL版" },
                new List<string> { "magazineTags", "COMIC" },
                new List<string> { "comiketsessionTags", "C" },
                new List<string> { "translatorTags_Chinese", "漢化", "中国語", "汉化", "中国翻訳" },
                new List<string> { "translatorTags_English", "英訳" },
                new List<string>{"authorTags", "国崎蛍" }
                };
                foreach (var list in vs)
                {
                    try
                    {
                        var temp = databases.TagKeywords.Single(n => n.TagName == list[0]);
                    }
                    catch (InvalidOperationException ex)
                    {
                        string tagname = list[0];
                        list.RemoveAt(0);
                        var one = EntityFactory.TagKeywordsFactory.Creat(tagname, list);
                        databases.TagKeywords.Add(one);
                    }
                }
                await databases.SaveChangesAsync();
            }
        }
    }
}