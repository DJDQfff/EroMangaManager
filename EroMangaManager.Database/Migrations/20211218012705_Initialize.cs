using Microsoft.EntityFrameworkCore.Migrations;

namespace EroMangaManager.Database.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageFilterTable",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(nullable: true),
                    ZipEntryLength = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageFilterTable", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReadingRecords",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AbsolutePath = table.Column<string>(nullable: true),
                    PageAmount = table.Column<int>(nullable: false),
                    ReadingPosition = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReadingRecords", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageFilterTable");

            migrationBuilder.DropTable(
                name: "ReadingRecords");
        }
    }
}
