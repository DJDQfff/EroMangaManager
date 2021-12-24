using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using EroMangaManager.Database.Entities;

namespace EroMangaManager.Database.Tables
{
    public class Databases : DbContext
    {
        public DbSet<ImageFilter> ImageFilterTable { set; get; }

        private readonly string ConnectionString;

        public Databases () => ConnectionString = "Data Source=Database.db";

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(ConnectionString);
        }
        protected override void OnModelCreating (ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}