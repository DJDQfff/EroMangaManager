using Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;

public class SearchFilterViewModelFactory(IServiceProvider sp)
{
    /// <summary>Bookcase：单组，固定源，无筛选时显示全部</summary>
    public SearchFilterViewModel CreateForGroup(MangasGroup group) =>
        new(() => group.Mangas)
        {
            ShowAllWhenNoCondition = true,
            IsPaged = true,
            PageSize = 20,
        };

    /// <summary>GlobalSearch：全局，实时源，无筛选时返回空</summary>
    public SearchFilterViewModel CreateForGlobal()
    {
        var vm = sp.GetRequiredService<ObservableCollectionVM>();
        return new(() => vm.MangaList.ToList()) { ShowAllWhenNoCondition = false, IsPaged = false };
    }
}
