using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaTagDatabase.Entities;
using EroMangaTagDatabase.Helper;

namespace EroMangaTagDatabase.EntityFactory
{
    public static class ReadingInfoFactory
    {
        public static ReadingInfo Creat (string absolutepath)
        {
            string manganamewithTag = Path.GetFileNameWithoutExtension(absolutepath);

            var tags = manganamewithTag.SplitAndParser();
            string manganame = tags[0] ?? manganamewithTag;      // 如果第一个元素为null，则以本子全名为名
            tags.RemoveAt(0);

            ReadingInfo readingInfo = new ReadingInfo()
            {
                AbsolutePath = absolutepath,
                MangaName = manganame,
                MangaName_Translated = manganame,// 未翻译的情况下，直接以原名作为翻译名，省去了判断是否翻译的麻烦

                TagPieces = string.Join("\r", tags),
                ReadingPosition = 0
            };

            return readingInfo;
        }
    }
}