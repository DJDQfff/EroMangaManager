using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.DatabaseOperation;
using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.Tables;

using Microsoft.EntityFrameworkCore;

namespace EroMangaTagDatabase.DatabaseOperation
{
    public static class DatabaseInitialize
    {
        /// <summary>
        /// 数据库迁移，并初始化默认数据
        /// </summary>
        public static async Task InitializeDefaultData ()
        {
            using (Database databases = new Database())
            {
                // 版本迁移
                databases.Database.Migrate();

                // 初始化数据
                List<string>[] vs = new List<string>[]
                {
                new List<string> { TagType.fullColorTags.ToString(), "全彩" },
                new List<string>{TagType.fullColorTags.ToString(),"无修", "無修", "无码","無码" },
                new List<string> { TagType.downloadversionTags.ToString(), "DL版" },
                new List<string> { TagType.magazineTags.ToString(), "COMIC" },
                new List<string> { TagType.comiketsessionTags.ToString(), "C" },
                new List<string> { TagType.translatorTags_Chinese.ToString(), "漢化", "中国語", "汉化", "中国翻訳" },
                new List<string> { TagType.translatorTags_English.ToString(), "英訳" },
                new List<string>{TagType.authorTags.ToString(), "国崎蛍" },
                new   List<string>{TagType.mangalongTags.ToString(),"长篇","单行本"},
                new List<string>{TagType.mangashortTags.ToString(),"短篇"},
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