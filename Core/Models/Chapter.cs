

namespace EroMangaManager.Core.Models;

public class Chapter(Manga manga) : IEnumerable<Stream>
{
    /// <summary>
    /// 源manga
    /// </summary>
   // [System.Text.Json.Serialization.JsonIgnore]
    public Manga Manga { get; init; } = manga;

    public string chaptername { get; init; }
    public IEnumerable<string> chapterimagekey { get; init; }

    public IEnumerator<Stream> GetEnumerator()
    {
        foreach (var str in chapterimagekey)
        {
            switch (Manga.Type)
            {
                case "":
                    {
                        yield return new FileStream(str, FileMode.Open, FileAccess.Read);

                    }
                    break;
                default:
                    {
                        var archive = ArchiveFactory.Open(Manga.FilePath);
                        var entry = archive.Entries.SingleOrDefault(x => x.Key == str);
                        yield return entry.OpenEntryStream();
                    }
                    break;
            }

        }

    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
