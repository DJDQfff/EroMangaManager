using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MyInternetAPI.BaiduTranslate.Models.ResponseJSON;

using static EroMangaDB.BasicController;

namespace EroMangaManager.UWP.Helpers
{
    /// <summary>
    /// 翻译器
    /// </summary>
    public class Translator
    {
        /// <summary> 翻译多个本子名 </summary>
        /// <returns> </returns>
        public static async Task TranslateAllMangaName ()
        {
            var names = App.Current.GlobalViewModel.MangaList.Select(n => n.MangaName).ToList();

            List<trans_result> results = null;

            using (MyInternetAPI.BaiduTranslate.Translator controller = new MyInternetAPI.BaiduTranslate.Translator())
            {
                try
                {
                    results = await controller.CommonTranslateAsync(names , "zh");
                }
                catch (Exception)
                {
                    // 翻译出错
                }
            }

            if (results != null)
            {
                //List<(string, string)> translateTuples = new List<(string, string)>();

                foreach (var manga in App.Current.GlobalViewModel.MangaList)
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