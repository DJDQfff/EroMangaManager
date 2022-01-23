namespace EroMangaTagDatabase.Entities
{
    public class ReadingInfo : IDatabaseID
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary>
        /// 绝对路径，作为唯一性标志
        /// </summary>
        public string AbsolutePath { set; get; }

        /// <summary>
        /// 本子名，剔除诸多Tag之后的名称（一般未包含在括号里的）
        /// </summary>
        public string MangaName { set; get; }

        /// <summary>
        /// 翻译过后的MangaName
        /// </summary>
        public string TranslatedMangaName { set; get; }

        /// <summary> 漫画总页数 </summary>
        public int PageAmount { get; set; }

        /// <summary> 当前阅读位置（进度） </summary>
        public int ReadingPosition { get; set; }

        /// <summary>
        /// 以\r分割的各个标签（不包括本子名）
        /// </summary>
        public string TagPieces { set; get; }
    }
}