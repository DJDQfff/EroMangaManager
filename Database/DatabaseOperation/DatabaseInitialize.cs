using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Database.Entities;
using Database.EntityFactory;
using SharpCompress.Archives;
using SharpCompress.Readers;

namespace Database;

public partial class DatabaseController
{
    /// <summary> 并初始化默认数据 </summary>
    public async Task InitializeDefaultData()
    {
        using var database = contextFactory.CreateDbContext();
        // 初始化数据
        List<string>[] vs =
        [
            [PresetTagCategory.全彩.ToString(), "全彩"],
            [PresetTagCategory.无修.ToString(), "无修", "无修正", "無修正", "無修", "无码", "無码"],
            [PresetTagCategory.DL版.ToString(), "DL版"],
            [PresetTagCategory.刊登.ToString(), "COMIC"],
            [PresetTagCategory.CM展.ToString(), "C99"],
            [PresetTagCategory.中译.ToString(), "漢化", "中国語", "汉化", "中国翻訳"],
            [PresetTagCategory.英译.ToString(), "英訳"],
            [PresetTagCategory.作者.ToString(), "国崎蛍"],
            [PresetTagCategory.单行本.ToString(), "长篇", "单行本"],
            [PresetTagCategory.短篇.ToString(), "短篇"],
        ];
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
                var one = TagCategoryFactory.Creat(tagname, list);
                database.TagCategorys.Add(one);
            }
        }
        await database.SaveChangesAsync();
    }

    public string LoadCategoryFromAssembly(string enbededResourceFileName)
    {
        var assembly = typeof(DatabaseController).Assembly;
        var assemblyName = assembly.GetName().Name; // "EroMangaDatabase" 或其他实际项目名
        var zip = assembly.GetManifestResourceStream(
            $"{assemblyName}.{enbededResourceFileName}.7z"
        );
        ReaderOptions readerOptions = new() { Password = "F9429775-6EAB-48FC-9F8A-4E079F90AF3F" };
        var stream = new MemoryStream();
        zip!.Position = 0;
        var archive = ArchiveFactory.OpenArchive(zip, readerOptions);

        foreach (var item in archive.Entries)
        {
            item.WriteTo(stream);
            stream.Position = 0;
            break;
        }
        StreamReader sr = new(stream);
        var a = sr.ReadToEnd();
        sr.Close();
        return a;
    }
}
