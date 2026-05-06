namespace EroMangaDatabase.Entities
{
    /// <summary>
    /// 添加的文件夹
    /// </summary>
    public class MangaFolder : IDatabaseID
    {
        /// <summary>
        ///
        /// </summary>
        public int ID { set; get; }

        /// <summary>
        ///
        /// </summary>
        public string Path { set; get; }
    }
}