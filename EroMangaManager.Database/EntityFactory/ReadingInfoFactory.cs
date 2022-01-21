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

            string[] tags = manganamewithTag.SplitAndParser();

            ReadingInfo readingInfo = new ReadingInfo() { AbsolutePath = absolutepath, MangaName = tags[0], ReadingPosition = 0 };

            return readingInfo;
        }
    }
}