using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilteredImages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ZipEntryLength = table.Column<long>(type: "INTEGER", nullable: false),
                    Hash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredImages", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MangaFolders",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MangaFolders", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReadingInfos",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AbsolutePath = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Name_Translated = table.Column<string>(type: "TEXT", nullable: false),
                    PageAmount = table.Column<int>(type: "INTEGER", nullable: false),
                    ReadingPosition = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsAddedByUser = table.Column<string>(type: "TEXT", nullable: false),
                    IsContentTranslated = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingInfos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TagCategorys",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(type: "TEXT", nullable: false),
                    Keywords = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCategorys", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UWPAccessIStorages",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Path = table.Column<string>(type: "TEXT", nullable: false),
                    IsFileOrFolder = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UWPAccessIStorages", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilteredImages");

            migrationBuilder.DropTable(
                name: "MangaFolders");

            migrationBuilder.DropTable(
                name: "ReadingInfos");

            migrationBuilder.DropTable(
                name: "TagCategorys");

            migrationBuilder.DropTable(
                name: "UWPAccessIStorages");
        }
    }
}
