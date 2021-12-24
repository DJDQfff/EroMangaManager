using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Database.Entities;
namespace EroMangaManager.Database.Utility
{
    public static class MangaTagInfoFactory
    {
        public static MangaTagInfo Creat (string absolutePath)
        {
            MangaTagInfo mangaTagInfo = new MangaTagInfo();
            mangaTagInfo.SetAbsolutePath(absolutePath);
            mangaTagInfo.SetDisplayName();
           mangaTagInfo.canbePair();
            if (mangaTagInfo.PaisIsDouble)
            {
                mangaTagInfo.SplitTags(mangaTagInfo.DisplayName);
            }
            return mangaTagInfo;

        }
        /// <summary> 判断本子是否无修及是否全彩 </summary>
        private static void SetNonMosaicAndColor (this MangaTagInfo mangaTagInfo)
        {
            string[] tags = mangaTagInfo.UnknownTags.Split('\r');
            foreach(var tag in tags)
            {
                if (tag.Contains("无修") || tag.Contains("無修"))
                {
                    mangaTagInfo.NonMosaic = true;
                }

                if (tag.Contains("全彩"))
                {
                    mangaTagInfo.IsFullColor = true;
                }

            }
        }

        public static void canbePair(this MangaTagInfo mangaTagInfo)
        {
            char[] chars1 = { '[', '【', '（', '(', '{' };// 左括号
            char[] chars2 =  { ']', '】', '）', ')', '}' };// 右括号
            char[][] chars = { chars1, chars2 };

            for(int i = 0; i < 5; i++)
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
        public static void SplitTags (this MangaTagInfo mangaTagInfo, string tags)
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
                        index2 = tags.IndexOf(']',index1);
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
                if (index2 !=-1)   // 左括号有对应的右括号
                {
                    string tag = tags.Substring(index1 + 1, index2 - index1 - 1);
                    mangaTagInfo.UnknownTags += "\r" + tag.Trim();
                    tags = tags.Remove(index1, index2 - index1+1);
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
                        mangaTagInfo.UnknownTags += "\r" + tags.Trim();
                    }
                }
            }

        }
        public static void SetDisplayName (this MangaTagInfo mangaTagInfo)
        {
            string path = mangaTagInfo.AbsolutePath;
            string DisplayName = Path.GetFileNameWithoutExtension(path);
            mangaTagInfo.DisplayName = DisplayName;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mangaTagInfo"></param>
        /// <param name="absolutePath"></param>
        public static void SetAbsolutePath (this MangaTagInfo mangaTagInfo, string absolutePath)
        {
            mangaTagInfo.AbsolutePath = absolutePath;
        }


    }
}