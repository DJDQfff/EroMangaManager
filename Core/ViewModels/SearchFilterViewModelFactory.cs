using Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ViewModels;

/// <summary>
///
/// </summary>
/// <param name="sp"></param>
/// <param name="coverSetter"></param>
public class SearchFilterViewModelFactory(IServiceProvider sp, CoverSetter coverSetter)
{
    /// <summary>Bookcase：单组，固定源，无筛选时显示全部</summary>
    public SearchFilterViewModel CreateForGroup(MangasGroup group) =>
        new(() => group.Mangas, coverSetter)
        {
            ShowAllWhenNoCondition = true,
            IsPaged = true,
            PageSize = 20,
        };

    /// <summary>GlobalSearch：全局，实时源，无筛选时返回空</summary>
    public SearchFilterViewModel CreateForGlobal()
    {
        var vm = sp.GetRequiredService<ObservableCollectionVM>();
        return new(() => [.. vm.MangaList], coverSetter)
        {
            ShowAllWhenNoCondition = false,
            IsPaged = false,
        };
    }
}
