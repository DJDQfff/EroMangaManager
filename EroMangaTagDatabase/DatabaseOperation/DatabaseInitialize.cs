using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using EroMangaDB.Entities;

namespace EroMangaDB
{
    public partial class BasicController
    {
        /// <summary> 并初始化默认数据 </summary>
        public async Task InitializeDefaultData ()
        {
            // 初始化数据
            List<string>[] vs = new List<string>[]
            {
                new List<string> { PresetTagCategory.全彩.ToString(), "全彩" },
                new List<string>{PresetTagCategory.无修.ToString(),"无修","无修正","無修正", "無修", "无码","無码" },
                new List<string> { PresetTagCategory.DL版.ToString(), "DL版" },
                new List<string> { PresetTagCategory.刊登.ToString(), "COMIC" },
                new List<string> { PresetTagCategory.CM展.ToString(), "C99" },
                new List<string> { PresetTagCategory.中译.ToString(), "漢化", "中国語", "汉化", "中国翻訳" },
                new List<string> { PresetTagCategory.英译.ToString(), "英訳" },
                new List<string>{PresetTagCategory.作者.ToString(), "国崎蛍" },
                new   List<string>{PresetTagCategory.单行本.ToString(),"长篇","单行本"},
                new List<string>{PresetTagCategory.短篇.ToString(),"短篇"},
            };
            foreach (var list in vs)
            {
                try
                {
                    var temp = database.TagCategorys.Single(n => n.CategoryName == list[0]);
                }
                catch (InvalidOperationException)
                {
                    string tagname = list[0];
                    list.RemoveAt(0);
                    var one = EntityFactory.TagCategoryFactory.Creat(tagname , list);
                    database.TagCategorys.Add(one);
                }
            }
            await database.SaveChangesAsync();
        }
    }
}