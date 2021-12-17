﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EroMangaManager.Database.Entities;
using EroMangaManager.Database.Tables;
using EroMangaManager.Database.Tools;
using EroMangaManager.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EroMangaManager.Models
{
    public static class HashManager
    {
        public static bool LengthFilter (long length)
        {
            int count = HashOperation.LengthConditionCount(length);
            return count == 0 ? true : false;
        }

        public static bool StreamHashFilter (Stream stream)
        {
            string hash = stream.ComputeHash();
            int count = HashOperation.HashConditionCount(hash);
            return count == 0 ? true : false;
        }

        public static void Add (string hash)
        {
            HashOperation.Add(hash);
        }

        public static void Remove (string hash)
        {
            HashOperation.Remove(hash);
        }
    }
}