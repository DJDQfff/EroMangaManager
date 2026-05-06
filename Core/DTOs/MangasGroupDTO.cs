using EroMangaManager.Core.ViewModels;
using System.Collections.ObjectModel;

namespace EroMangaManager.Core.DTOs
{
    public class MangasGroupDTO
    {
        public string Guid { get; set; }
        //public  ObservableCollection<MangaDTO> MangaDTOs { get; set; } = [];

        public string Name { get; set; }
        //public List<string> Tags { get; set; } = [];   

        public MangasGroupDTO() { }
        public MangasGroupDTO( MangasGroup folder)
        {
            Guid = folder.Guid;
            Name = Path.GetFileNameWithoutExtension(folder.FolderPath);

        }

    }
    }
