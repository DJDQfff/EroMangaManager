using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.Tables;

using Microsoft.EntityFrameworkCore;

namespace EroMangaTagDatabase
{
    public partial class BasicController
    {
        /// <summary> 数据库迁移，并初始化默认数据 </summary>
        public async Task InitializeDefaultData ()
        {
            // 版本迁移
            database.Database.Migrate();

            // 初始化数据
            List<string>[] vs = new List<string>[]
            {
                new List<string> { DefaultTagType.全彩.ToString(), "全彩" },
                new List<string>{DefaultTagType.无修.ToString(),"无修","无修正","無修正", "無修", "无码","無码" },
                new List<string> { DefaultTagType.DL版.ToString(), "DL版" },
                new List<string> { DefaultTagType.刊登.ToString(), "COMIC" },
                new List<string> { DefaultTagType.CM展.ToString(), "C99" },
                new List<string> { DefaultTagType.中译.ToString(), "漢化", "中国語", "汉化", "中国翻訳" },
                new List<string> { DefaultTagType.英译.ToString(), "英訳" },
                new List<string>{DefaultTagType.作者.ToString(), "国崎蛍" },
                new   List<string>{DefaultTagType.单行本.ToString(),"长篇","单行本"},
                new List<string>{DefaultTagType.短篇.ToString(),"短篇"},
            };
            foreach (var list in vs)
            {
                try
                {
                    var temp = database.TagKeywords.Single(n => n.TagName == list[0]);
                }
                catch (InvalidOperationException)
                {
                    string tagname = list[0];
                    list.RemoveAt(0);
                    var one = EntityFactory.TagKeywordsFactory.Creat(tagname, list);
                    database.TagKeywords.Add(one);
                }
            }
            await database.SaveChangesAsync();
        }
    }
}