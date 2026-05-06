namespace EroMangaDatabase.Tables
{
    /// <summary>
    /// 配置内容
    /// </summary>
    public static class DatabaseConfig
    {
        /// <summary>
        /// 数据库连接字符串。只能设置一次
        /// </summary>
        public static string ConnectingString { set; get; }
    }
}