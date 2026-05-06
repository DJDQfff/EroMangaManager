using EroMangaManager.Core.Models;

namespace EroMangaManager.Core.DTOs
{
    public class MangaDTO
    {
        public string Guid { get; set; }

        public string Name { set; get; }
        public string CoverUri { get; set; }
        public MangaDTO() { }
        public MangaDTO(Manga manga)
        {
            Guid = manga.Guid;
            Name = manga.MangaName;
        }
    }
}
