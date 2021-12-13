namespace EroMangaManager.Database.Entities
{
    public class Record
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> 绝对路径，用于判断文件唯一性 </summary>
        public string AbsolutePath { set; get; }

        /// <summary> 漫画总页数 </summary>
        public int PageAmount { get; set; }

        /// <summary> 当前阅读位置（进度） </summary>
        public int ReadingPosition { get; set; }
    }
}