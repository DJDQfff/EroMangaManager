namespace EroMangaDatabase.Entities
{
    /// <summary>
    /// uwp访问权限存储
    /// </summary>
    public class UWPAccessIStorage : IDatabaseID
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 指示这个路径是文件还是文件夹，为true则为文件夹，问false则为文件
        /// </summary>
        public bool IsFileOrFolder { set; get; }

        /// <summary>
        /// uwp的访问权限token
        /// </summary>
        public string AccessToken { set; get; }
    }
}