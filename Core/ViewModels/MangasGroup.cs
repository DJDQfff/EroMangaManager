using System.Diagnostics.CodeAnalysis;
using Core.ViewModels;

namespace Core.ViewModels;

public partial class MangasGroup : ObservableObject
{
    /// <summary>
    ///
    /// </summary>
    public string FolderPath { get; set; } = string.Empty;

    /// <summary>
    ///
    /// </summary>
    public string Guid { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MangasGroupUpdateState UpdateState { get; set; } = MangasGroupUpdateState.Ready;

    /// <summary>
    ///
    /// </summary>
    public List<Manga> Mangas { get; set; } = [];

    /// <summary>
    /// 源数据总数（全局展示用，比如 LibraryPage）。
    /// 不受筛选影响；筛选后数量见 SearchFilterViewModel.FilteredCount。
    /// </summary>
    [ObservableProperty]
    public partial int Count { get; set; }

    /// <summary>
    ///
    /// </summary>
    public MangasGroup() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="storageFolderPath"></param>
    [SetsRequiredMembers]
    public MangasGroup(string storageFolderPath) => FolderPath = storageFolderPath;

    /// <summary>
    ///
    /// </summary>
    /// <param name="mangas"></param>
    /// <returns></returns>
    public int AddManga(params IList<Manga> mangas)
    {
        Mangas.AddRange(mangas);
        Count = Mangas.Count;
        return Count;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="mangaBook"></param>
    /// <returns></returns>
    public bool RemoveManga(Manga mangaBook)
    {
        bool removed = Mangas.Remove(mangaBook);
        if (removed)
            Count = Mangas.Count;
        return removed;
    }
}
