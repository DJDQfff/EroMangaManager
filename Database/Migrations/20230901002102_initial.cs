using Microsoft.EntityFrameworkCore.Migrations;

namespace EroMangaDatabase.Migrations
{
#pragma warning disable CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。

    public partial class initial : Migration
#pragma warning restore CS8981 // 该类型名称仅包含小写 ascii 字符。此类名称可能会成为该语言的保留值。
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilteredImages",
                columns: table =>
                    new
                    {
                        ID = table
                            .Column<int>(nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        ZipEntryLength = table.Column<long>(nullable: false),
                        Hash = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredImages", x => x.ID);
                }
            );

            migrationBuilder.CreateTable(
                name: "MangaFolders",
                columns: table =>
                    new
                    {
                        ID = table
                            .Column<int>(nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        Path = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaFolders", x => x.ID);
                }
            );

            migrationBuilder.CreateTable(
                name: "ReadingInfos",
                columns: table =>
                    new
                    {
                        ID = table
                            .Column<int>(nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        AbsolutePath = table.Column<string>(nullable: true),
                        MangaName = table.Column<string>(nullable: true),
                        MangaName_Translated = table.Column<string>(nullable: true),
                        PageAmount = table.Column<int>(nullable: false),
                        ReadingPosition = table.Column<int>(nullable: false),
                        TagsAddedByUser = table.Column<string>(nullable: true),
                        IsContentTranslated = table.Column<bool>(nullable: false)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingInfos", x => x.ID);
                }
            );

            migrationBuilder.CreateTable(
                name: "TagCategorys",
                columns: table =>
                    new
                    {
                        ID = table
                            .Column<int>(nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        CategoryName = table.Column<string>(nullable: true),
                        Keywords = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCategorys", x => x.ID);
                }
            );

            migrationBuilder.CreateTable(
                name: "UWPAccessIStorages",
                columns: table =>
                    new
                    {
                        ID = table
                            .Column<int>(nullable: false)
                            .Annotation("Sqlite:Autoincrement", true),
                        Path = table.Column<string>(nullable: true),
                        IsFileOrFolder = table.Column<bool>(nullable: false),
                        AccessToken = table.Column<string>(nullable: true)
                    },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UWPAccessIStorages", x => x.ID);
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FilteredImages");

            migrationBuilder.DropTable(name: "MangaFolders");

            migrationBuilder.DropTable(name: "ReadingInfos");

            migrationBuilder.DropTable(name: "TagCategorys");

            migrationBuilder.DropTable(name: "UWPAccessIStorages");
        }
    }
}