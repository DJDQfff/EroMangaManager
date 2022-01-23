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
            MangaTag mangaTagInfo = new MangaTag()
            {
                IsFullColor = false,                    // 默认黑白
                IsDL = false,                           // 默认非DL版
                Language = "Japanese",                  // 默认日语
                NonMosaic = false,                      // 默认有码
                PaisIsDouble = true,                    // 默认括号成对
                LongShort = false                       // 默认短篇
            };

            mangaTagInfo.AbsolutePath = absolutePath;

            mangaTagInfo.DisplayName = Path.GetFileNameWithoutExtension(absolutePath);

            var tags = mangaTagInfo.DisplayName.SplitAndParser();

            mangaTagInfo.MangaName = tags[0];

            mangaTagInfo.ParseTags(tags);

            return mangaTagInfo;
        }

        private static void ParseTags (this MangaTag mangaTagInfo, List<string> tags)
        {
            string[] fullColorTags = TagKeywordsOperation.QueryTagKeywords("fullColorTags");
            string[] nonMosaicTags = TagKeywordsOperation.QueryTagKeywords("nonMosaicTags");
            string[] downloadversionTags = TagKeywordsOperation.QueryTagKeywords("downloadversionTags");
            string[] magazineTags = TagKeywordsOperation.QueryTagKeywords("magazineTags");
            string[] comiketsessionTags = TagKeywordsOperation.QueryTagKeywords("comiketsessionTags");
            string[] translatorTags_Chinese = TagKeywordsOperation.QueryTagKeywords("translatorTags_Chinese");
            string[] translatorTags_English = TagKeywordsOperation.QueryTagKeywords("translatorTags_English");
            string[] authorTags = TagKeywordsOperation.QueryTagKeywords("authorTags");
            string[] mangalongTags = TagKeywordsOperation.QueryTagKeywords("mangalongTags");
            string[] mangashortTags = TagKeywordsOperation.QueryTagKeywords("mangashortTags");

            for (int i = 1; i < tags.Count; i++)
            {
                string tag = tags[i];

                // 判断本子作者
                if (tag.ParseInclude(authorTags))
                {
                    mangaTagInfo.Author = tag;
                    continue;
                }

                // 判断是否长篇
                if (tag.ParseInclude(mangalongTags))
                {
                    mangaTagInfo.LongShort = true;
                    continue;
                }

                // 判断是否短篇
                if (tag.ParseInclude(mangashortTags))
                {
                    mangaTagInfo.LongShort = false;
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
                    mangaTagInfo.MagazinePublished = tag;
                    continue;
                }
                //判断CM展会信息
                if (tag.ParseInclude(comiketsessionTags))// 初步判断
                {
                    mangaTagInfo.MagazinePublished = tag;
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