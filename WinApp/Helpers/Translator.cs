using DJDQfff.BaiduTranslateAPI;
using DJDQfff.BaiduTranslateAPI.Models.ResponseJSON;

namespace EroMangaManager.WinUI3.Helpers
{
    /// <summary>
    /// 翻译器
    /// </summary>
    public class Translator
    {
        /// <summary> 翻译多个本子名 </summary>
        /// <returns> </returns>
        public static async Task TranslateAllMangaName()
        {
            var names = App.Current.GlobalViewModel.MangaList.Select(n => n.MangaName).ToList();

            List<trans_result> results = null;

            using (
                var controller = new SimpleTranslator("20210219000701366", "VkerV4o1qG1TK6mUlbr_")
            )
            {
                try
                {
                    results = await controller.CommonTextTranslateAsync(names);
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
                    string newname = results
                        .Where(n => n.src == manga.MangaName)
                        ?.FirstOrDefault()
                        ?.dst;
                    if (newname != null)
                    {
                        manga.TranslatedMangaName = newname;
                        //translateTuples.Add((Manga.FilePath, newname));
                    }
                }

                // 找到了，在生成ReadingInfo时，就已经添加到datavase了，所以上面直接修改TranslatedMangaName会被EFCore跟踪修改
                //await DatabaseController.ReadingInfo_MultiTranslateMangaName(translateTuples);
            }
        }
    }
}