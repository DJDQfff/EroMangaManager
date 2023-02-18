using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using EroMangaDB.Entities;
using MyStandard20Library;
namespace EroMangaDB.Services
{
    public class TagCategoryManager
    {
        public ObservableCollection<TagCategory> AllTagCategory { get; set; } = new ObservableCollection<TagCategory>();



    }
}
