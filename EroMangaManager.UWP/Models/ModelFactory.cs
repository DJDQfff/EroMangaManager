using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using EroMangaDB.Helper;

using EroMangaManager.Core.Models;
using EroMangaManager.UWP.Helpers;
using EroMangaManager.UWP.Views.MainPageChildPages;

using Windows.Storage;

namespace EroMangaManager.UWP.Models
{
    internal static class ModelFactory
    {
        public static async Task<MangaBook> CreateMangaBook (StorageFile storageFile , StorageFolder storageFolder)
        {
             var filepath = storageFile.Path;
            MangaBook mangaBook = new MangaBook ( filepath);

            var coverpath = await Helpers.CoverHelper.TryCreatCoverFileAsync(storageFile);
            mangaBook.CoverPath = coverpath ?? CoverHelper.DefaultCoverPath;

            return mangaBook;

        }
    }
}
