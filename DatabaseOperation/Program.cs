using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.EntityFactory;
using EroMangaTagDatabase.Helper;
using EroMangaTagDatabase.Tables;
using static EroMangaTagDatabase.Controller;
using Microsoft.EntityFrameworkCore;

namespace DatabaseOperation
{
    internal class Program
    {
        private static async Task Main ()
        {
            await DatabaseController.InitializeDefaultData();
            var a = DatabaseController.TagKeywords_QueryAll();

            await DatabaseController.TagKeywords_AddTagSingle("fjakjf", new string[] { "djfa", "jfds" });

            var b = DatabaseController.TagKeywords_QueryAll();

            DatabaseController.Dispose();
            System.Console.ReadKey();
        }

        private static async Task Test1 ()
        {
            Database databases = new Database();
            databases.Database.Migrate();
            var readingInfo1 = ReadingInfoFactory.Creat(@"D:\桌面\新建文件夹\新建文件夹\[aki99] 彼女を起こさないで (Fatekaleid liner プリズマ☆イリヤ) [中国語] [無修正].zip");
            var readingInfo = ReadingInfoFactory.Creat(@"D:\桌面\新建文件夹\新建文件夹\(C95) [毛玉牛乳 (玉之けだま)] 甘リリス (オリジナル) [DL版][山樱汉化][全彩][无修正].zip");

            List<ReadingInfo> list = new List<ReadingInfo>();
            list.Add(readingInfo);
            list.Add(readingInfo1);
        }

        private static void TagTest ()
        {
            string tags = @"(C95) [毛玉牛乳 (玉之けだま)] 甘リリス (オリジナル) [DL版][山樱汉化][全彩][无修正]";
            //string tag1 = string.Empty;
            //string tag2 = string.Empty;
            //tags.TagsSpliter(ref tag2, ref tag1);
            //System.Console.WriteLine(tag2);
            //System.Console.WriteLine(tag1);

            var a = tags.SplitAndParser();

            foreach (var tag in a)
            {
                System.Console.WriteLine(tag);
            }
        }
    }
}