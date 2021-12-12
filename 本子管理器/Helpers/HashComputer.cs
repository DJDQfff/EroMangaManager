using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using EroMangaManager.Models;

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
    }
}