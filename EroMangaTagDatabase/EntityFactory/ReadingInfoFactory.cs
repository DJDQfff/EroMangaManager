using System.IO;

using EroMangaDB.Entities;
using EroMangaDB.Helper;

namespace EroMangaDB.EntityFactory
{
    /// <summary>
    /// 阅读信息类工厂方法
    /// </summary>
    public static class ReadingInfoFactory
    {
        /// <summary>
        /// 创建一个新的阅读状态信息
        /// </summary>
        /// <param name="absolutepath"></param>
        /// <returns></returns>
        public static ReadingInfo Creat (string absolutepath)
        {
            string manganamewithTag = Path.GetFileNameWithoutExtension(absolutepath);

            var tags = manganamewithTag.SplitAndParser();
            string manganame = tags[0] ?? manganamewithTag;      // 如果第一个元素为null，则以本子全名为名
            tags.RemoveAt(0);

            ReadingInfo readingInfo = new ReadingInfo()
            {
                AbsolutePath = absolutepath ,
                MangaName = manganame ,
                MangaName_Translated = manganame ,// 未翻译的情况下，直接以原名作为翻译名，省去了判断是否翻译的麻烦

                TagsAddedByUser = string.Join("\r" , tags) ,
                ReadingPosition = 0
            };

            return readingInfo;
        }
    }
}