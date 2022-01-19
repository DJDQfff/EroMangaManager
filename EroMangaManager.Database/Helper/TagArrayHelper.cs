using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using EroMangaManager.Database.Entities;

namespace EroMangaManager.Database.Helper
{
    public static class TagArrayHelper
    {
        public static bool canbePair (this string tagstring)
        {
            char[] chars1 = { '[', '【', '（', '(', '{' };// 左括号
            char[] chars2 = { ']', '】', '）', ')', '}' };// 右括号
            char[][] chars = { chars1, chars2 };

            for (int i = 0; i < 5; i++)
            {
                int count1 = tagstring.Count(n => n == chars[0][i]);
                int count2 = tagstring.Count(n => n == chars[1][i]);
                if (count1 != count2)
                {
                    return false;
                }
            }
            return true;
        }

        public static string[] SplitAndParser (this string tagstring)
        {
            string left = "[【（({";// 左括号
            string right = "]】)）}";// 右括号
            string lr = left + right;// 所有括号

            string[] tagsarray = tagstring.Split(lr.ToCharArray());              // 初步分解

            List<string> tagslist = new List<string>(tagsarray);

            for (int i = tagslist.Count - 1; i != -1; i--)          // 移除所有为空白的tag
            {
                if (string.IsNullOrWhiteSpace(tagslist[i]))
                    tagslist.RemoveAt(i);
            }

            foreach (var tag in tagslist)                           // 查找漫画名Tag
            {                                                       // 查找方法：该Tag左边为无括号或者为右括号
                int index = tagstring.IndexOf(tag);                 //          等价于：左边不是左括号
                char c = tagstring[index - 1];
                if (!left.Contains(c))
                {
                    tagslist.Remove(tag);                           // 调整位置，把漫画名Tag移到集合头部
                    tagslist.Insert(0, tag);
                    break;
                }
            }

            for (int i = 0; i < tagslist.Count; i++)                // 移除所有Tag的首位空白
            {
                tagslist[i] = tagslist[i].Trim();
            }

            string[] finaltags = tagslist.ToArray();
            return finaltags;
        }

        public static bool ParseInclude (this string _unknownTag, IEnumerable<string> tags)
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