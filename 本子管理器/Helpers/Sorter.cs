using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EroMangaManager.Helpers
{
    public static class Sorter
    {
        public static void SortEntries (this IList<ZipArchiveEntry> zipArchiveEntries)
        {
        }

        private static string SameLenth (this string str, int manl)
        {
            string newstr = str.PadLeft(manl, '0');
            return newstr;
        }

        private static int ManLength (this IList<string> list)
        {
            int[] lengthlist = list.Select(n => n.Length).ToArray();
            int max = lengthlist.Max();

            return max;
        }

        private static string FrontName (this string name)
        {
            int index = name.LastIndexOf('.');
            string front = name.Substring(0, index);
            return front;
        }
    }
}