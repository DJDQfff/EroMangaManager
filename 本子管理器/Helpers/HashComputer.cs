using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;

namespace EroMangaManager.Helpers
{
    public static class HashComputer
    {
        public static string ComputeHash (this Stream stream)
        {
            SHA256 sHA256 = SHA256.Create();
            byte[] vs = sHA256.ComputeHash(stream);
            string hash = BitConverter.ToString(vs).Replace("-", "");
            sHA256.Dispose();
            return hash;
        }

        public static string ComputeHash (this ZipArchiveEntry entry)
        {
            string hash;
            using (Stream stream = entry?.Open())
            {
                hash = stream.ComputeHash();
            }
            return hash;
        }
    }
}