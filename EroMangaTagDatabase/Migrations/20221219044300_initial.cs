using Microsoft.EntityFrameworkCore.Migrations;

namespace EroMangaDB.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilteredImages",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ZipEntryLength = table.Column<long>(nullable: false),
                    Hash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilteredImages", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ManTagInfos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AbsolutePath = table.Column<string>(nullable: true),
                    PaisIsDouble = table.Column<bool>(nullable: false),
                    DisplayName = table.Column<string>(nullable: true),
                    MangaName = table.Column<string>(nullable: true),
                    IsFullColor = table.Column<bool>(nullable: false),
                    Language = table.Column<string>(nullable: true),
                    NonMosaic = table.Column<bool>(nullable: false),
                    IsDL = table.Column<bool>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    Translator = table.Column<string>(nullable: true),
                    MagazinePublished = table.Column<string>(nullable: true),
                    CM_session = table.Column<string>(nullable: true),
                    Relative_ACG = table.Column<string>(nullable: true),
                    UnknownTags = table.Column<string>(nullable: true),
                    LongShort = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManTagInfos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReadingInfos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AbsolutePath = table.Column<string>(nullable: true),
                    MangaName = table.Column<string>(nullable: true),
                    MangaName_Translated = table.Column<string>(nullable: true),
                    PageAmount = table.Column<int>(nullable: false),
                    ReadingPosition = table.Column<int>(nullable: false),
                    TagPieces = table.Column<string>(nullable: true),
                    IsContentTranslated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingInfos", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TagCategorys",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryName = table.Column<string>(nullable: true),
                    Keywords = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCategorys", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilteredImages");

            migrationBuilder.DropTable(
                name: "ManTagInfos");

            migrationBuilder.DropTable(
                name: "ReadingInfos");

            migrationBuilder.DropTable(
                name: "TagCategorys");
        }
    }
}
