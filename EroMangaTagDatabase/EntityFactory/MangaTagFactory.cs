//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Text;

//using EroMangaTagDatabase.Helper;
//using EroMangaTagDatabase.Entities;
//using EroMangaTagDatabase.DatabaseOperation;

//namespace EroMangaTagDatabase.EntityFactory
//{
//    /// <summary>
//    /// 已弃用
//    /// </summary>
//    public static class MangaTagFactory
//    {
//        public static DefaultMangaTag Creat (string absolutePath)
//        {
//            DefaultMangaTag mangaTagInfo = new DefaultMangaTag()
//            {
//                IsFullColor = false,                    // 默认黑白
//                IsDL = false,                           // 默认非DL版
//                Language = "Japanese",                  // 默认日语
//                NonMosaic = false,                      // 默认有码
//                PaisIsDouble = true,                    // 默认括号成对
//                LongShort = false                       // 默认短篇
//            };

//            mangaTagInfo.AbsolutePath = absolutePath;

//            mangaTagInfo.DisplayName = Path.GetFileNameWithoutExtension(absolutePath);

//            var tags = mangaTagInfo.DisplayName.SplitAndParser();

//            mangaTagInfo.MangaName = tags[0];

//            mangaTagInfo.ParseTags(tags);

//            return mangaTagInfo;
//        }

//        private static void ParseTags (this DefaultMangaTag mangaTagInfo, List<string> tags)
//        {
//            string[] fullColorTags = TagKeywordsOperation.QuerySingleTagKeywords(DefaultTagType.全彩.ToString());
//            string[] nonMosaicTags = TagKeywordsOperation.QuerySingleTagKeywords(DefaultTagType.无修.ToString());
//            string[] downloadversionTags = TagKeywordsOperation.QuerySingleTagKeywords("downloadversionTags");
//            string[] magazineTags = TagKeywordsOperation.QuerySingleTagKeywords("magazineTags");
//            string[] comiketsessionTags = TagKeywordsOperation.QuerySingleTagKeywords("comiketsessionTags");
//            string[] translatorTags_Chinese = TagKeywordsOperation.QuerySingleTagKeywords("translatorTags_Chinese");
//            string[] translatorTags_English = TagKeywordsOperation.QuerySingleTagKeywords("translatorTags_English");
//            string[] authorTags = TagKeywordsOperation.QuerySingleTagKeywords("authorTags");
//            string[] mangalongTags = TagKeywordsOperation.QuerySingleTagKeywords("mangalongTags");
//            string[] mangashortTags = TagKeywordsOperation.QuerySingleTagKeywords("mangashortTags");

//            for (int i = 1; i < tags.Count; i++)
//            {
//                string tag = tags[i];

//                // 判断本子作者
//                if (tag.ParseInclude(authorTags))
//                {
//                    mangaTagInfo.Author = tag;
//                    continue;
//                }

//                // 判断是否长篇
//                if (tag.ParseInclude(mangalongTags))
//                {
//                    mangaTagInfo.LongShort = true;
//                    continue;
//                }

//                // 判断是否短篇
//                if (tag.ParseInclude(mangashortTags))
//                {
//                    mangaTagInfo.LongShort = false;
//                    continue;
//                }

//                // 判断本子是否全彩
//                if (tag.ParseInclude(fullColorTags))
//                {
//                    mangaTagInfo.IsFullColor = true;
//                    continue;
//                }
//                // 判断本子是否无修
//                if (tag.ParseInclude(nonMosaicTags))
//                {
//                    mangaTagInfo.NonMosaic = true;
//                    continue;
//                }
//                // 判断本子是否是DL版
//                if (tag.ParseInclude(downloadversionTags))
//                {
//                    mangaTagInfo.IsDL = true;
//                    continue;
//                }
//                // 判断是否是杂志
//                if (tag.ParseInclude(magazineTags))
//                {
//                    mangaTagInfo.MagazinePublished = tag;
//                    continue;
//                }
//                //判断CM展会信息
//                if (tag.ParseInclude(comiketsessionTags))// 初步判断
//                {
//                    mangaTagInfo.MagazinePublished = tag;
//                    continue;
//                }

//                if (tag.ParseInclude(translatorTags_Chinese))
//                {
//                    mangaTagInfo.Translator = tag;
//                    mangaTagInfo.Language = "Chinese";
//                    continue;
//                }
//                if (tag.ParseInclude(translatorTags_English))
//                {
//                    mangaTagInfo.Translator = tag;
//                    mangaTagInfo.Language = "English";
//                    continue;
//                }
//            }
//        }
//    }
//}