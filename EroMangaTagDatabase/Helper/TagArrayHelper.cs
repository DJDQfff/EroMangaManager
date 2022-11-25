using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using EroMangaDB.Entities;

namespace EroMangaDB.Helper
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

        public static List<string> SplitAndParser (this string tagstring)
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

            bool flaghaveMangaName = false;

            try
            {
                foreach (var tag in tagslist)                           // 查找漫画名Tag
                {
                    int index = tagstring.IndexOf(tag);
                    char c = tagstring[index - 1];// 这里要改一下，不能直接用左边一位
                    if (!left.Contains(c))                              // 查找方法：该Tag左边为无括号或者为右括号
                    {                                                   //          等价于：左边不是左括号
                        tagslist.Remove(tag);                           // 如果存在，则调整位置，把漫画名Tag移到集合头部
                        tagslist.Insert(0, tag);
                        flaghaveMangaName = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
                // 出现异常说明： 漫画名不是由Tag组成的、或者组成形式不符合一般规律
            }

            for (int i = 0; i < tagslist.Count; i++)                // 移除所有Tag的首尾空白
            {
                tagslist[i] = tagslist[i].Trim();
            }

            if (!flaghaveMangaName)
            {
                tagslist.Insert(0, null);                             // 如果不存在MangaName，则以空字符串为默认名
            }

            return tagslist;
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