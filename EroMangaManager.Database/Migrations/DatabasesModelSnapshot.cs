﻿// <auto-generated />
using EroMangaTagDatabase.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EroMangaTagDatabase.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabasesModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.21");

            modelBuilder.Entity("EroMangaTagDatabase.Entities.ImageFilter", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Hash")
                        .HasColumnType("TEXT");

                    b.Property<long>("ZipEntryLength")
                        .HasColumnType("INTEGER");

                    b.HasKey("ID");

                    b.ToTable("ImageFilters");
                });

            modelBuilder.Entity("EroMangaTagDatabase.Entities.MangaTag", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AbsolutePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("Author")
                        .HasColumnType("TEXT");

                    b.Property<string>("CM_session")
                        .HasColumnType("TEXT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDL")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsFullColor")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<string>("MagazinePublished")
                        .HasColumnType("TEXT");

                    b.Property<string>("MangaName")
                        .HasColumnType("TEXT");

                    b.Property<bool>("NonMosaic")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("PaisIsDouble")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Relative_ACG")
                        .HasColumnType("TEXT");

                    b.Property<string>("Translator")
                        .HasColumnType("TEXT");

                    b.Property<string>("UnknownTags")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("SpecificMangaTagDatas");
                });

            modelBuilder.Entity("EroMangaTagDatabase.Entities.ReadingInfo", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("AbsolutePath")
                        .HasColumnType("TEXT");

                    b.Property<string>("MangaName")
                        .HasColumnType("TEXT");

                    b.Property<int>("PageAmount")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReadingPosition")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TranslatedMangaName")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("ReadingRecords");
                });

            modelBuilder.Entity("EroMangaTagDatabase.Entities.TagKeywords", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("TagKeywordPieces")
                        .HasColumnType("TEXT");

                    b.Property<string>("TagName")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("TagKeywords");
                });
#pragma warning restore 612, 618
        }
    }
}
