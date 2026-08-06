using Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace Database;

/// <summary>
/// 将DBContext实例包装在这个类里面
/// </summary>
/// <remarks>
/// 构造函数注入工厂
/// </remarks>
public partial class DatabaseController(IDbContextFactory<DataBase_Version3> contextFactory)
{
    /// <summary>
    /// 数据库版本迁移
    /// </summary>
    public void Migrate()
    {
        using var database = contextFactory.CreateDbContext();
        database.Database.Migrate();
    }
}
