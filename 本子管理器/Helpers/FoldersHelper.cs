using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace EroMangaManager.Helpers
{
    public static class FoldersHelper
    {
        public static async Task EnsureFolders ()
        {
            StorageFolder LocalCacheFolder = ApplicationData.Current.TemporaryFolder;

            _ = await LocalCacheFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
            _ = await LocalCacheFolder.CreateFolderAsync("Filter", CreationCollisionOption.OpenIfExists);
        }

        public static async Task<StorageFolder> GetCoversFolder ()
        {
            StorageFolder LocalCacheFolder = ApplicationData.Current.TemporaryFolder;

            StorageFolder CoversFolder = await LocalCacheFolder.GetFolderAsync("Covers");

            return CoversFolder;
        }

        public static async Task<StorageFolder> GetFilterFolder ()
        {
            StorageFolder LocalCacheFolder = ApplicationData.Current.TemporaryFolder;

            StorageFolder FiltersFolder = await LocalCacheFolder.GetFolderAsync("Filter");

            return FiltersFolder;
        }
    }
}