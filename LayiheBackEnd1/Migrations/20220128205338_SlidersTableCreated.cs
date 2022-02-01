using Microsoft.EntityFrameworkCore.Migrations;

namespace LayiheBackEnd.Migrations
{
    public partial class SlidersTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sliders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubTitle = table.Column<string>(maxLength: 150, nullable: true),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    Descrption = table.Column<string>(maxLength: 300, nullable: true),
                    BackImage = table.Column<string>(nullable: true),
                    BtnUrl = table.Column<string>(maxLength: 250, nullable: true),
                    BtnText = table.Column<string>(maxLength: 35, nullable: true),
                    Order = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sliders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sliders");
        }
    }
}
