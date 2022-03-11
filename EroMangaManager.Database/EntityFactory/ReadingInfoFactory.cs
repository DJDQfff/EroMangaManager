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

            var tags = manganamewithTag.SplitAndParser();        // TODO 存在Bug，名称可能不是标准的由Tag组成的文件名
            string manganame = tags[0] ?? manganamewithTag;
            tags.RemoveAt(0);

            ReadingInfo readingInfo = new ReadingInfo()
            {
                AbsolutePath = absolutepath,
                MangaName = manganame, // 如果第一个元素为null，则以本子全名为名
                TagPieces = string.Join("\r", tags),
                ReadingPosition = 0
            };

            return readingInfo;
        }
    }
}