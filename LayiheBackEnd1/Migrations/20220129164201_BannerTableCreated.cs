using Microsoft.EntityFrameworkCore.Migrations;

namespace LayiheBackEnd.Migrations
{
    public partial class BannerTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title1 = table.Column<string>(maxLength: 200, nullable: true),
                    Title2 = table.Column<string>(maxLength: 200, nullable: true),
                    SubTitle = table.Column<string>(maxLength: 200, nullable: true),
                    BackImage = table.Column<string>(maxLength: 200, nullable: true),
                    RedirectUrl = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banners", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banners");
        }
    }
}
