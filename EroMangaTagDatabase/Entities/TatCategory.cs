namespace EroMangaDB.Entities
{
    /// <summary>
    /// tag种类
    /// 设计目的：
    /// 如：初音未来、镜音双子这两个tag都是一个种类：V家
    ///     中国翻译、靴下汉化这两个tag都是一个种类：Translator（这是一个PresetTag）
    /// 所以这两个都是tag都作为一个TagKeywords的两个TagKeywords，同时指定一个TagName作为唯一名
    /// </summary>
    public class TatCategory : IDatabaseID
    {
        /// <summary> 主键 </summary>
        public int ID { set; get; }

        /// <summary> 自定义Tag种类名称 </summary>
        public string CategoryName { set; get; }

        /// <summary>
        /// 自定义Tag的各个关键字，每个关键字以‘\r’分割，每个关键字都是该tag的识别标志之一
        /// 第一个：对外显示的
        /// </summary>
        public string Keywords { set; get; }
    }
}