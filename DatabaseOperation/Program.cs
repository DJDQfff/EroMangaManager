using System;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Tables;
using EroMangaManager.Database.Utility;

namespace DatabaseOperation
{
    internal class Program
    {
        private static void Main ()
        {
            string path = @"D:\收藏\本子\无修\(C95) [毛玉牛乳 (玉之けだま)] 甘リリス (オリジナル) [DL版][山樱汉化][全彩][无修正].zip";
            MangaTagInfo mangaTagInfo = MangaTagInfoFactory.Creat(path);
            

            Console.WriteLine("Hello World!");
        }
    }
}