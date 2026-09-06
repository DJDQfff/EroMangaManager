namespace UnoLibrary;

public record NavigationItem(Type Page, StringsEnum Uid, string UidValue, SymbolIcon Icon);

public interface IPages
{
    // 优化点 1：使用 Append 替代 Concat 和数组创建，减少内存分配
    // 优化点 2：使用 KeyNotFoundException 替代通用 Exception
    // 优化点 3：使用 is not null 模式匹配，逻辑更清晰
    NavigationItem Find(StringsEnum uid)
    {
        var item = MainPages
            .Concat(FooterPages)
            .Append(SettingPage) // Append 是扩展方法，比 Concat([x]) 更轻量
            .SingleOrDefault(x => x.Uid == uid);

        if (item is not null)
        {
            return item;
        }

        throw new KeyNotFoundException($"未找到对应的导航页面: {uid}");
    }

    NavigationItem[] MainPages { get; }
    NavigationItem[] FooterPages { get; }
    NavigationItem SettingPage { get; }
}
