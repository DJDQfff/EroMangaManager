using System;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Utility;

namespace DatabaseOperation
{
    internal class Program
    {
        private static void Main ()
        {
            string folderPath = @"D:\收藏\本子\无修";
            string[] pathes = System.IO.Directory.GetFiles(folderPath);
            foreach (var path in pathes)
            {
                MangaTag mangaTagInfo = MangaTagFactory.Creat(path);

                Console.WriteLine("Hello World!");
            }
        }
    }
}