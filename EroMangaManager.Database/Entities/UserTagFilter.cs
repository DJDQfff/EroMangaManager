namespace EroMangaManager.Database.Entities
{
    public class UserTagFilter
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> Tag类型名 </summary>
        public string TagType { set; get; }

        /// <summary> 此类型的Tag数组，以 '\r' 分割 </summary>
        public string TagArray { set; get; }
    }
}