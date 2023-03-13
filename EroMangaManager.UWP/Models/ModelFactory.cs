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

using iText.Layout.Font;

using Windows.Storage;

namespace EroMangaManager.UWP.Models
{
    internal static class ModelFactory
    {
        public static async Task<MangaBook> CreateMangaBook (StorageFile storageFile , StorageFolder storageFolder)
        {
// TODO 这个区域的内容都可以丢到mangabook类实例里面去，同时把对应属性改为只可内部修改，外部只读
            #region
            MangaBook mangaBook = new MangaBook ();
            mangaBook.FilePath = storageFile.Path;
            mangaBook.FolderPath=storageFolder.Path;

            var tags =mangaBook. FileDisplayName.SplitAndParser();
            mangaBook.MangaName = tags[0];
            mangaBook. MangaTagsIncludedInFileName = tags.Skip(1).ToArray();
            #endregion

            var coverpath = await Helpers.CoverHelper.TryCreatCoverFileAsync(storageFile);
            mangaBook.CoverPath = coverpath ?? CoverHelper.DefaultCoverPath;

            return mangaBook;

        }
    }
}
