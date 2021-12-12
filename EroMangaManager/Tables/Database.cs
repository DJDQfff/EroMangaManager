using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using EroMangaManager.Database.Entities;

namespace EroMangaManager.Database.Tables
{
    public class Database : DbContext
    {
        public DbSet<ImageHash> imageHashes { set; get; }
        public DbSet<Record> ReadingRecords { set; get; }

        private readonly string ConnectionString;

        public Database () => ConnectionString = "Data Source=Database.db";

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}