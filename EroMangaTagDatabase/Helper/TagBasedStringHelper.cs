﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace EroMangaDB.Helper
{
    /// <summary>
    /// 对基于tag的字符串的相关操作的帮助整合类
    /// </summary>
    public static class TagBasedStringHelper
    {
        private const string left = "[【（({";// 左括号
        private const string right = "]】)）}";// 右括号
        private const string leftright = left + right;// 所有括号

        /// <summary>
        /// 按左右括号分离tag并解析
        /// </summary>
        /// <param name="_MangaFileName">传入漫画文件名（不带后缀）</param>
        /// <returns>第一个是MangaName，后面的是tag</returns>
        public static List<string> SplitAndParser(this string _MangaFileName)
        {
            string[] tagsarray = _MangaFileName.Split(leftright.ToCharArray());              // 初步分解

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
                    int index = _MangaFileName.IndexOf(tag);
                    char c = _MangaFileName[index - 1];// 这里要改一下，不能直接用左边一位
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

            tagslist.ForEach(x => x.Trim());            // 移除所有Tag的首尾空白

            if (!flaghaveMangaName)
            {
                tagslist.Insert(0, _MangaFileName);          // 如果不存在MangaName，则以文件名为本子名
            }

            return tagslist;
        }

        /// <summary>
        ///移除其中包含的重复tag
        /// </summary>
        /// <param name="oldname">旧的名字</param>
        public static string RemoveRepeatTag(string oldname)
        {
            // TODO 输入一个包含重复tag的名称，算出一个去掉重复tag的名称
            return null;
        }

        /// <summary>
        /// 是否左右括号成对
        /// </summary>
        /// <param name="tagstring"></param>
        /// <returns></returns>
        public static bool CanbePair(this string tagstring)
        {
            for (int i = 0; i < 5; i++)
            {
                int count1 = tagstring.Count(n => n == left[i]);
                int count2 = tagstring.Count(n => n == right[i]);
                if (count1 != count2)
                {
                    return false;
                }
            }
            return true;
        }
    }
}