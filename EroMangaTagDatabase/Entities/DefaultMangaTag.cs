namespace EroMangaDB.Entities
{
    public class DefaultMangaTag : IDatabaseID
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> 绝对路径，用于判断文件唯一性 </summary>
        public string AbsolutePath { set; get; }

        /// <summary> 括号是否成双对 </summary>
        public bool PaisIsDouble { set; get; }

        /// <summary> 本子名（不带扩展名） </summary>
        public string DisplayName { set; get; }

        /// <summary> 本子名（移除tag之后） </summary>
        public string MangaName { set; get; }

        /// <summary> 是否全彩，true是全彩，默认为false </summary>
        public bool IsFullColor { set; get; }

        /// <summary> 语言,日，中，英 </summary>
        public string Language { set; get; }

        /// <summary> 是否无码 </summary>
        public bool NonMosaic { set; get; }

        /// <summary> 图源是扫描还是DL版（电子版） </summary>
        public bool IsDL { set; get; }

        /// <summary> 画师名 </summary>
        public string Author { set; get; }

        /// <summary> 翻译作者 </summary>
        public string Translator { set; get; }

        /// <summary> 连载的杂志 </summary>
        public string MagazinePublished { set; get; }

        /// <summary> 第 x 届CM展作品 </summary>
        public string CM_session { set; get; }

        /// <summary> 是xxx的同人作品 </summary>
        public string Relative_ACG { set; get; }

        /// <summary> 未分类标签集 </summary>
        public string UnknownTags { set; get; }

        /// <summary>
        /// 长篇还是短篇，长篇为1
        /// </summary>
        public bool LongShort { set; get; }
    }
}