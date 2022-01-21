using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using BaiduTranslate;
using EroMangaTagDatabase.DatabaseOperation;

namespace EroMangaManager.Models
{
    public class Translater
    {
        public static async Task TranslateAllMangaName ()
        {
            var names = MainPage.current.collectionObserver.MangaList.Select(n => n.MangaName).ToList();

            Controller controller = new Controller();
            var results = await controller.CommonTranslateAsync(names, "zh");

            List<(string, string)> tuples = new List<(string, string)>();

            foreach (var manga in MainPage.current.collectionObserver.MangaList)
            {
                string newname = results.Where(n => n.src == manga.MangaName)?.FirstOrDefault()?.dst;
                if (newname != null)
                {
                    manga.TranslateMangaName(newname);
                    tuples.Add((manga.StorageFile.Path, newname));
                }
            }

            await ReadingInfoTableOperation.MultiTranslateMangaName(tuples);
        }
    }
}