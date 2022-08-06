using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using EroMangaTagDatabase.Entities;

namespace Abandoned.EroMangaTagDatabase.EntityFactory
{
    public static class MangaTagFactory
    {
        public static DefaultMangaTag Creat (string absolutePath)
        {
            DefaultMangaTag mangaTagInfo = new DefaultMangaTag() { IsFullColor = false, IsDL = false, Language = "Janpanese", NonMosaic = false, PaisIsDouble = true };
            mangaTagInfo.SetAbsolutePath(absolutePath);
            mangaTagInfo.SetDisplayName();
            mangaTagInfo.canbePair();
            if (mangaTagInfo.PaisIsDouble)
            {
                mangaTagInfo.SplitTags(mangaTagInfo.DisplayName);
                mangaTagInfo.ParseTags();
            }
            return mangaTagInfo;
        }

        private static void ParseTags (this DefaultMangaTag mangaTagInfo)
        {
            string[] tags = mangaTagInfo.UnknownTags.Split('\r');
            string[] fullCollarTags = { "全彩" };
            string[] nonMosaicTags = { "无修", "無修" };
            string[] dlTags = { "DL版" };
            string[] magazineTags = { "COMIC" };
            string[] cmTags = { "C" };
            string[] translatorTags_Chinese = { "漢化", "中国語", "汉化", "中国翻訳" };
            string[] translatorTags_English = { "英訳" };

            foreach (string tag in tags)
            {
                // 判断本子是否全彩
                if (tag.ParseInclude(fullCollarTags))
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
                if (tag.ParseInclude(dlTags))
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
                if (tag.ParseInclude(cmTags))// 初步判断
                {
                    string cmVersion = tag;
                    foreach (var t in cmTags)
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

        public static void canbePair (this DefaultMangaTag mangaTagInfo)
        {
            char[] chars1 = { '[', '【', '（', '(', '{' };// 左括号
            char[] chars2 = { ']', '】', '）', ')', '}' };// 右括号
            char[][] chars = { chars1, chars2 };

            for (int i = 0; i < 5; i++)
            {
                int count1 = mangaTagInfo.DisplayName.Count(n => n == chars[0][i]);
                int count2 = mangaTagInfo.DisplayName.Count(n => n == chars[1][i]);
                if (count1 != count2)
                {
                    mangaTagInfo.PaisIsDouble = false;
                    return;
                }
            }
            mangaTagInfo.PaisIsDouble = true;
        }

        public static void SplitTags (this DefaultMangaTag mangaTagInfo, string tags)
        {
            char[] chars1 = new char[] { '[', '【', '（', '(', '{' };// 左括号
            char[] chars2 = new char[] { ']', '】', ')', '）', '}' };// 右括号

            tags = tags.Trim();
            int index1 = tags.IndexOfAny(chars1);
            if (index1 != -1)       // tags中以左括号开始
            {
                int index2 = default;
                switch (tags[index1])
                {
                    case '[':
                        index2 = tags.IndexOf(']', index1);
                        break;

                    case '【':
                        index2 = tags.IndexOf('】', index1);
                        break;

                    case '（':
                        index2 = tags.IndexOf('）', index1);
                        break;

                    case '(':
                        index2 = tags.IndexOf(')', index1);
                        break;

                    case '{':
                        index2 = tags.IndexOf('}', index1);
                        break;
                }
                if (index2 != -1)   // 左括号有对应的右括号
                {
                    string tag = tags.Substring(index1 + 1, index2 - index1 - 1);
                    mangaTagInfo.UnknownTags += "\r" + tag.Trim();
                    tags = tags.Remove(index1, index2 - index1 + 1);
                    mangaTagInfo.SplitTags(tags);
                }
            }
            else                    // 没有从左括号开始
            {
                int index0 = tags.IndexOfAny(chars2);
                if (index0 == -1)   // 既没有左括号也没有右括号
                {
                    if (tags.Length != 0)   //   非空，说明这是一段没用括号括起来的有效tag（一般为本子名）
                    {
                        mangaTagInfo.MangaName = tags.Trim();
                        mangaTagInfo.UnknownTags += "\r" + tags.Trim();
                    }
                }
            }
        }

        public static void SetDisplayName (this DefaultMangaTag mangaTagInfo)
        {
            string path = mangaTagInfo.AbsolutePath;
            string DisplayName = Path.GetFileNameWithoutExtension(path);
            mangaTagInfo.DisplayName = DisplayName;
        }

        /// <summary> 初始化 </summary>
        /// <param name="mangaTagInfo"> </param>
        /// <param name="absolutePath"> </param>
        public static void SetAbsolutePath (this DefaultMangaTag mangaTagInfo, string absolutePath)
        {
            mangaTagInfo.AbsolutePath = absolutePath;
        }

        private static bool ParseInclude (this string _unknownTag, IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (_unknownTag.Contains(tag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}