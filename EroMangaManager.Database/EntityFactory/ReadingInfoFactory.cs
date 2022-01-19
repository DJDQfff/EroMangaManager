using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Helper;

namespace EroMangaManager.Database.EntityFactory
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