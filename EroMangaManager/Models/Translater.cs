using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MyInternetAPI.BaiduTranslate;
using MyInternetAPI.BaiduTranslate.Models.ResponseJSON;
using static EroMangaTagDatabase.BasicController;
using System;

namespace EroMangaManager.Models
{
    public class Translater
    {
        /// <summary> 翻译多个本子名 </summary>
        /// <returns> </returns>
        public static async Task TranslateAllMangaName ()
        {
            var names = MainPage.current.collectionObserver.MangaList.Select(n => n.MangaName).ToList();

            List<trans_result> results = null;

            using (Translator controller = new Translator())
            {
                try
                {
                    results = await controller.CommonTranslateAsync(names, "zh");
                }
                catch (Exception)
                {
                    // 翻译出错
                }
            }

            if (results != null)
            {
                //List<(string, string)> translateTuples = new List<(string, string)>();

                foreach (var manga in MainPage.current.collectionObserver.MangaList)
                {
                    string newname = results.Where(n => n.src == manga.MangaName)?.FirstOrDefault()?.dst;
                    if (newname != null)
                    {
                        manga.TranslatedMangaName = newname;
                        //translateTuples.Add((manga.FilePath, newname));
                    }
                }

                // 找到了，在生成ReadingInfo时，就已经添加到datavase了，所以上面直接修改TranslatedMangaName会被EFCore跟踪修改
                await DatabaseController.SaveChanges();
                //await DatabaseController.ReadingInfo_MultiTranslateMangaName(translateTuples);
            }
        }
    }
}