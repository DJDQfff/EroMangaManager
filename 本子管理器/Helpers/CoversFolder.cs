using System;
using System.Threading.Tasks;

using Windows.Storage;

namespace EroMangaManager.Helpers
{
    public static class CoversFolder
    {
        public static async Task EnsureCreat ()
        {
            StorageFolder LocalCacheFolder = ApplicationData.Current.TemporaryFolder;

            _ = await LocalCacheFolder.CreateFolderAsync("Covers", CreationCollisionOption.OpenIfExists);
        }

        public static async Task<StorageFolder> Get ()
        {
            StorageFolder LocalCacheFolder = ApplicationData.Current.TemporaryFolder;

            StorageFolder CoversFolder = await LocalCacheFolder.GetFolderAsync("Covers");

            return CoversFolder;
        }
    }
}