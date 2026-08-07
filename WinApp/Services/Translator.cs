using DJDQfff.BaiduTranslateAPI;
using DJDQfff.BaiduTranslateAPI.Models.ResponseJSON;

namespace UnoLibrary.Services;

/// <summary>
/// 翻译器
/// </summary>
public class Translator(ObservableCollectionVM collectionVM)
{
    /// <summary> 翻译多个本子名 </summary>
    /// <returns> </returns>
    public async Task TranslateAllName()
    {
        var names = collectionVM.MangaList.Select(n => n.Name);

        List<trans_result> results = null!;

        using (var controller = new SimpleTranslator("20210219000701366", "VkerV4o1qG1TK6mUlbr_"))
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

            foreach (var manga in collectionVM.MangaList)
            {
                var newname = results.Where(n => n.src == manga.Name)?.FirstOrDefault()?.dst;
                if (newname != null)
                {
                    manga.TranslatedName = newname;
                    //translateTuples.Add((Manga.FilePath, newname));
                }
            }

            // 找到了，在生成ReadingInfo时，就已经添加到datavase了，所以上面直接修改TranslatedName会被EFCore跟踪修改
            //await DatabaseController.ReadingInfo_MultiTranslateName(translateTuples);
        }
    }
}
