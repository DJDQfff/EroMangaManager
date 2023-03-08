using System.Collections.ObjectModel;

using EroMangaDB.Entities;

namespace EroMangaDB.Services
{
    public class TagCategoryManager
    {
        public ObservableCollection<TagCategory> AllTagCategory { get; set; } = new ObservableCollection<TagCategory>();
    }
}