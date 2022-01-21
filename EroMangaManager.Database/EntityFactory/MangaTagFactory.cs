using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaTagDatabase.Helper;
using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.DatabaseOperation;
namespace EroMangaTagDatabase.EntityFactory
{
    public static class MangaTagFactory
    {
        public static MangaTag Creat (string absolutePath)
        {
            MangaTag mangaTagInfo = new MangaTag() { IsFullColor = false, IsDL = false, Language = "Japanese", NonMosaic = false, PaisIsDouble = true };
            mangaTagInfo.AbsolutePath = absolutePath;

            mangaTagInfo.DisplayName = Path.GetFileNameWithoutExtension(absolutePath);

            string[] tags = mangaTagInfo.DisplayName.SplitAndParser();

            mangaTagInfo.MangaName = tags[0];

            mangaTagInfo.ParseTags(tags);

            return mangaTagInfo;
        }
        private static void ParseTags (this MangaTag mangaTagInfo, string[] tags)
        {
            string[] fullColorTags = TagKeywordsOperation.QueryTagKeywords("fullColorTags");
            string[] nonMosaicTags = TagKeywordsOperation.QueryTagKeywords("nonMosaicTags");
            string[] downloadversionTags = TagKeywordsOperation.QueryTagKeywords("downloadversionTags");
            string[] magazineTags = TagKeywordsOperation.QueryTagKeywords("magazineTags");
            string[] comiketsessionTags = TagKeywordsOperation.QueryTagKeywords("comiketsessionTags");
            string[] translatorTags_Chinese = TagKeywordsOperation.QueryTagKeywords("translatorTags_Chinese");
            string[] translatorTags_English = TagKeywordsOperation.QueryTagKeywords("translatorTags_English");
            string[] authorTags = TagKeywordsOperation.QueryTagKeywords("authorTags");
            for (int i = 1; i < tags.Length; i++)
            {
                string tag = tags[i];
                // 判断本子作者
                if (tag.ParseInclude(authorTags))
                {
                    mangaTagInfo.Author = tag;
                    continue;
                }
                // 判断本子是否全彩
                if (tag.ParseInclude(fullColorTags))
                {
                    mangaTagInfo.IsFullColor = true;
                    continue;
                }
                // 判断本子是否无修
                if (tag.ParseInclude(nonMosaicTags))
                {
                    mangaTagInfo.NonMosaic = true;
                    continue;
                }
                // 判断本子是否是DL版
                if (tag.ParseInclude(downloadversionTags))
                {
                    mangaTagInfo.IsDL = true;
                    continue;
                }
                // 判断是否是杂志
                if (tag.ParseInclude(magazineTags))
                {
                    string magazine = tag;
                    foreach (var t in magazineTags)
                    {
                        magazine = magazine.Replace(t, "");
                    }

                    mangaTagInfo.MagazinePublished = magazine;
                    continue;
                }
                //判断CM展会信息
                if (tag.ParseInclude(comiketsessionTags))// 初步判断
                {
                    string cmVersion = tag;
                    foreach (var t in comiketsessionTags)
                    {
                        cmVersion = cmVersion.Replace(t, "");
                    }

                    mangaTagInfo.MagazinePublished = cmVersion;
                    continue;
                }

                if (tag.ParseInclude(translatorTags_Chinese))
                {
                    mangaTagInfo.Translator = tag;
                    mangaTagInfo.Language = "Chinese";
                    continue;
                }
                if (tag.ParseInclude(translatorTags_English))
                {
                    mangaTagInfo.Translator = tag;
                    mangaTagInfo.Language = "English";
                    continue;
                }
            }
        }
    }
}