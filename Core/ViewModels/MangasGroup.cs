using System.Diagnostics.CodeAnalysis;
using Core.ViewModels;

namespace Core.ViewModels;

public partial class MangasGroup : ObservableObject
{
    public string FolderPath { get; set; } = string.Empty;
    public string Guid { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MangasGroupUpdateState UpdateState { get; set; } = MangasGroupUpdateState.Ready;

    public List<Manga> Mangas { get; set; } = [];

    /// <summary>
    /// 源数据总数（全局展示用，比如 LibraryPage）。
    /// 不受筛选影响；筛选后数量见 SearchFilterViewModel.FilteredCount。
    /// </summary>
    [ObservableProperty]
    public partial int Count { get; set; }

    public MangasGroup() { }

    [SetsRequiredMembers]
    public MangasGroup(string storageFolderPath) => FolderPath = storageFolderPath;

    public int AddManga(params IList<Manga> mangas)
    {
        Mangas.AddRange(mangas);
        Count = Mangas.Count;
        return Count;
    }

    public bool RemoveManga(Manga mangaBook)
    {
        bool removed = Mangas.Remove(mangaBook);
        if (removed)
            Count = Mangas.Count;
        return removed;
    }
}
