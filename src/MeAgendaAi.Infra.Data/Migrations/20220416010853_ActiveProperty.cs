using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class ActiveProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ACTIVE",
                table: "TB_USERS",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ACTIVE",
                table: "TB_USERS");
        }
    }
}
