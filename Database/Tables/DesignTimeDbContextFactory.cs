using Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Database.Tables
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataBase_Version3>
    {
        public DataBase_Version3 CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataBase_Version3>();

            // 设计时只需要一个固定的相对路径即可
            // EF Core 工具会在执行命令的目录下生成 design_time.db
            optionsBuilder.UseSqlite("Data Source=design_time.db");

            return new DataBase_Version3(optionsBuilder.Options);
        }
    }
}
