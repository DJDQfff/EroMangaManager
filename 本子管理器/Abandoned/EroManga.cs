using System;
using System.Collections.Generic;

/*
 * 这个版本初衷是为了从文件名开始把所有标签全解析出来
 * 但是很多本子乱加标签，即使识别了也无法
 * 一般具有的标签：C93、画师，无修、全彩、本子名
 * 奇奇怪怪的标签：MJK（无邪气）、aki99（类似于C95）、
 * 一般标签大部分可以解析、奇奇怪怪的标签实在没办法
 * 放弃这个类，写个简化功能的
 * 只获取本子名，是否无修，是否全彩
 */

using Windows.Storage;

/// <summary> Abandoned这个命名空间专门用来存放已放弃的 </summary>
namespace Abandoned.EroMangaManager.Models
{
    internal class EroManga
    {
        //本子名字
        public string name;

        //本子文件全名（不带扩展名）
        public string fullname;

        //作者,默认为空（不知道）
        public string author = string.Empty;

        //语言,日，中，英，默认为日语
        public string language = "japanese";

        //汉化者，默认无汉化者
        public string translator = string.Empty;

        //杂志信息
        public string comicinfo = string.Empty;

        //第几届CM展
        public string CMsession = string.Empty;

        //是否无修，true为有码，false为无码
        public bool isNonCovered = false;

        //是否DL版
        public bool isDLversion = false;

        //判断是否全彩
        public bool isFullColorful = false;

        //括号是否对称，true是对称，false为不对称，若不对称，不作多余操作
        public bool isPairs = true;

        public List<string> tags = new List<string>();

        /// <summary> 输入路径，获取信息 </summary>
        /// <param name="path"> </param>
        public EroManga (StorageFile storageFile)
        {
            fullname = storageFile.DisplayName.Trim();

            FindeTags(fullname);
            if (isPairs == true)
            {
                SetTags();
            }
        }

        private void SetTags ()
        {
            //遍历判断是否无修
            foreach (var item in tags)
            {
                string[] strs = { "无修", "無修" };
                if (item.Contains(strs[0]) || item.Contains(strs[1]))
                {
                    tags.Remove(item);
                    isNonCovered = true;
                    break;
                }
            }
            //遍历判断是否DL版
            foreach (var item in tags)
            {
                string str = "DL版";
                if (item.Contains(str))
                {
                    tags.Remove(item);
                    isDLversion = true;
                    break;
                }
            }
            //遍历判断是否全彩
            foreach (var item in tags)
            {
                string str = "全彩";
                if (item.Contains(str))
                {
                    tags.Remove(item);
                    isFullColorful = true;
                    break;
                }
            }

            //遍历获取杂志信息
            foreach (var item in tags)
            {
                string str = "COMIC";
                if (item.Contains(str))
                {//包含杂志信息
                    int index1 = 0;
                    int index2 = 0;
                    FindIndex(item, ref index1, ref index2);
                    if (index1 != 1 && index2 != -1)
                    {
                        string str1 = item.Substring(0, index1);//第一部分
                        string str2 = item.Replace(str1, string.Empty);
                        string str3 = RemoveBracket(str2.Trim());
                        comicinfo = str3.Trim();
                        tags.Remove(item);
                        break;
                    }
                }
            }
            //遍历获取汉化信息
            foreach (var item in tags)
            {
                string[] stra = { "漢化", "中国語", "汉化", "中国翻訳" };
                string strb = "英訳";
                //已汉化
                if (item.Contains(stra[0]) || item.Contains(stra[1]) || item.Contains(stra[2]) || item.Contains(stra[3]))
                {
                    int index1 = 0;
                    int index2 = 0;
                    FindIndex(item, ref index1, ref index2);
                    //含有name的特殊tag
                    if (item.Contains(name))
                    {
                        string str1 = item.Substring(0, index1);//在括号外的部分
                        string str2 = item.Replace(str1, string.Empty);
                        string str3 = RemoveBracket(str2.Trim());//包含在括号内的部分
                        language = "chinese";
                        translator = str3;
                        tags.Remove(item);
                        break;
                    }
                    else//一般的tag
                    {
                        translator = item;
                        language = "chinese";
                        tags.Remove(item);
                        break;
                    }
                }
                //已英译
                if (item.Contains(strb))
                {
                    int index1 = 0;
                    int index2 = 0;
                    FindIndex(item, ref index1, ref index2);
                    if (item.Contains(name))
                    {
                        string str1 = item.Substring(0, index1);//前部分
                        string str2 = item.Replace(str1, string.Empty);
                        string str3 = RemoveBracket(str2.Trim());
                        language = "english";
                        translator = str3;
                        tags.Remove(item);
                        break;
                    }
                    else//一般的tag
                    {
                        translator = item;
                        language = "english";
                        tags.Remove(item);
                        break;
                    }
                }
            }
            //遍历获取第几届CM展
            foreach (var item in tags)
            {
                if (item.IndexOf('C') == 0)
                {
                    string str1 = item.Substring(1, item.Length - 1);
                    try
                    {
                        int a = Convert.ToInt32(str1);
                        if (a != 0)
                        {
                            CMsession = item;
                            tags.Remove(item);
                            break;
                        }
                    }
                    catch (FormatException)
                    {
                        continue;
                    }
                }
            }
            //删除一些奇奇怪怪等等tag,及含有name的tag
            //如MJK-19-Z1637（无邪气的奇怪符号）、aki99（非通用cm届数说明）
            for (int i = 0; i <= tags.Count - 1; i++)
            {
                if (tags[i].Contains("aki"))
                { tags.Remove(tags[i]); continue; }
                if (tags[i].Contains("MJK"))
                { tags.Remove(tags[i]); continue; }
                if (tags[i].Contains(name))
                { tags.Remove(tags[i]); continue; }
            }
            //最后一个就是作者名（应该），如果还有没考虑的tag，以后再说咯
            //没有就没有作者了
            if (tags.Count == 1)
            {
                author = tags[0];
            }
        }

        /// <summary> 给tags录入数据，并获取本子名name </summary>
        /// <param name="str"> </param>
        private void FindeTags (string str)
        {
            if (str != string.Empty)
            {
                int index1 = 0;//左边的括号在哪里
                int index2 = 0;//右边的括号在哪里

                FindIndex(str, ref index1, ref index2);

                if (index1 != -1 && index2 != -1)//括号对称
                {
                    if (index1 != 0)   //第一个括号不在首位（前面有字不在括号内，一般为本子名）
                    {
                        string str0 = str.Substring(0, index2 + 1);
                        name = str.Substring(0, index1);//本子名
                        tags.Add(str0.Trim());

                        string str3 = str.Replace(str0, string.Empty).Trim();
                        FindeTags(str3);
                    }
                    else              //括号就在第一个
                    {
                        string str1 = str.Substring(index1, index2 + 1 - index1);
                        string str2 = RemoveBracket(str1).Trim();
                        tags.Add(str2);
                        string str3 = str.Replace(str1, string.Empty).Trim();
                        FindeTags(str3);
                    }
                }
                else
                {
                    isPairs = false;//不对称
                }
            }
        }

        /// <summary> 传入一个两端带括号的字符串，去掉括号 </summary>
        /// <param name="str"> </param>
        /// <returns> </returns>
        private string RemoveBracket (string str)
        {
            return str.Substring(1, str.Length - 2);
        }

        /// <summary> 获取左右括号的位置 </summary>
        /// <param name="str">    </param>
        /// <param name="index1"> </param>
        /// <param name="index2"> </param>
        private void FindIndex (string str, ref int index1, ref int index2)
        {
            char[] chars = { '(', ')', '[', ']', '（', '）', '【', '】' };
            int index0 = str.IndexOfAny(chars);

            char[] chars1 = { '[', ']', };
            char[] chars2 = { '【', '】' };
            char[] chars3 = { '（', '）' };
            char[] chars4 = { '(', ')' };

            if ((index1 = str.IndexOf(chars1[0])) == index0)
            {
                index2 = str.IndexOf(chars1[1], index1 + 1);
                return;
            }
            if ((index1 = str.IndexOf(chars2[0])) == index0)
            {
                index2 = str.IndexOf(chars2[1], index1 + 1);
                return;
            }
            if ((index1 = str.IndexOf(chars3[0])) == index0)
            {
                index2 = str.IndexOf(chars3[1], index1 + 1);
                return;
            }
            if ((index1 = str.IndexOf(chars4[0])) == index0)
            {
                index2 = str.IndexOf(chars4[1], index1 + 1);
                return;
            }

            //这个方法导致产生优先级，会按固定顺序查找下去，不能直接找到第一个
            //if ((index1 = str.IndexOf(chars1[0])) != -1)
            //{
            //    index2 = str.IndexOf(chars1[1], index1 + 1); return;
            //}
            //if ((index1 = str.IndexOf(chars2[0])) != -1)
            //{
            //    index2 = str.IndexOf(chars2[1], index1 + 1); return;
            //}
            //if ((index1 = str.IndexOf(chars3[0])) != -1)
            //{
            //    index2 = str.IndexOf(chars3[1], index1 + 1); return;
            //}
            //if ((index1 = str.IndexOf(chars4[0])) != -1)
            //{
            //    index2 = str.IndexOf(chars4[1], index1 + 1); return;
            //}
        }
    }
}