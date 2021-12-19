using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BaiduTranslate;

namespace EroMangaManager.Models
{
    public class Translater
    {
        public static async Task TranslateAllMangaName ()
        {
            var names = MainPage.currentMainPage.listObserver.MangaList.Select(n => n.MangaName).ToList();

            Controller controller = new Controller();
            var results = await controller.CommonTranslateAsync(names, "zh");
            foreach (var manga in MainPage.currentMainPage.listObserver.MangaList)
            {
                string newname = results.Where(n => n.src == manga.MangaName)?.FirstOrDefault()?.dst;
                if (newname != null)
                {
                    manga.SetTranslateName(newname);
                }
            }
        }
    }
}