namespace Core.Interfaces;

/// <summary>
/// Manga和MangaDTO类提取出的接口，用于供UnoLibrary控件库使用。已改成直接使用Manga，已不需要
/// </summary>
[Obsolete]
interface IMangaView : INotifyPropertyChanged
{
    string Name { get; }
    string CoverUri { get; }
    int ChapterAmount { get; }
    long FileSize { get; }
    int ImageAmount { get; }
    string[] Tags { get; }
}
