using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database.Tables
{
    /// <summary>
    /// 数据库类
    /// </summary>
    public class DataBase_Version3(DbContextOptions<DataBase_Version3> options) : DbContext(options)
    {
        // 你的 DbSet 保持不变
        public DbSet<FilteredImage> FilteredImages { set; get; }
        public DbSet<TagCategory> TagCategorys { set; get; }
        public DbSet<ReadingInfo> ReadingInfos { set; get; }
        public DbSet<MangaFolder> MangaFolders { set; get; }
        public DbSet<UWPAccessIStorage> UWPAccessIStorages { set; get; }

        // 注意：使用 AddDbContextFactory 时，OnConfiguring 方法可以直接删除！
        // 因为连接字符串已经在 Program.cs / App.xaml.cs 的 AddDbContextFactory 中配置好了
    }
}
