using System.Diagnostics.CodeAnalysis;
using Core.ViewModels;

namespace Core.DTOs;

/// <summary>
/// 简单Group映射
/// </summary>
public partial class MangasGroupDTO : ObservableObject
{
    /// <summary>
    /// 相同guid
    /// </summary>
    public required string Guid { get; set; }

    //public  ObservableCollection<MangaDTO> MangaDTOs { get; set; } = [];

    /// <summary>
    /// Name
    /// </summary>
    public required string Name { get; set; }

    //public List<string> Tags { get; set; } = [];
    /// <summary>
    ///
    /// </summary>
    [ObservableProperty]
    public partial int Count { set; get; }

    /// <summary>
    /// 无参构造，保留用于序列化
    /// </summary>
    public MangasGroupDTO() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="folder"></param>
    [SetsRequiredMembers]
    public MangasGroupDTO(MangasGroup folder)
    {
        Guid = folder.Guid;
        Name = Path.GetFileNameWithoutExtension(folder.FolderPath);
    }
}
