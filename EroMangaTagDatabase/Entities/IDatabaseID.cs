namespace EroMangaDB.Entities
{
    /// <summary>
    /// 数据库主键ID接口
    /// </summary>
    public interface IDatabaseID
    {
        /// <summary>
        /// 主键
        /// </summary>
        int ID { set; get; }
    }
}